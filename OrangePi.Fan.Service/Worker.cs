using Iot.Device.CpuTemperature;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Fan.Service.Models;

namespace OrangePi.Fan.Service
{

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<ServiceConfiguration> _serviceConfigMonitor;
        private readonly IProcessRunner _processRunner;
        private readonly ITemperatureReader _temperatureReader;
        private readonly IBuzzerService _buzzer;
        public Worker(
            ILogger<Worker> logger,
            IOptionsMonitor<ServiceConfiguration> serviceConfigMonitor,
            IProcessRunner processRunner,
            IEnumerable<ITemperatureReader> temperatureReader,
            IBuzzerService buzzer)
        {
            _logger = logger;
            _serviceConfigMonitor = serviceConfigMonitor;
            _processRunner = processRunner;
            _temperatureReader = temperatureReader.Single(s => s.GetType().Name == serviceConfigMonitor.CurrentValue.TemperatureReader ||
                                                               s.GetType().FullName == serviceConfigMonitor.CurrentValue.TemperatureReader);
            _buzzer = buzzer;

            _logger.LogInformation($"Initializing reader: {_temperatureReader.GetType().FullName}");
            _logger.LogInformation($"Reader check interval: {_serviceConfigMonitor.CurrentValue.TemperatureCheckIntervalSeconds} seconds");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_serviceConfigMonitor.CurrentValue.StartSound.Enabled)
                await _buzzer.Play(
                    frequency: 4000,
                    lenght: TimeSpan.FromSeconds(_serviceConfigMonitor.CurrentValue.StartSound.Interval))
                    .ConfigureAwait(false);

            double previousValue = int.MinValue;
            await _processRunner.RunAsync(command: "gpio", new string[] { "mode", _serviceConfigMonitor.CurrentValue.WiringPi.ToString(), "pwm" });

            while (!stoppingToken.IsCancellationRequested)
            {
                double temperature = 120;

                #region calculate ranges
                var thresholds = _serviceConfigMonitor.CurrentValue.TemperatureConfigurations.OrderBy(t => t.Temperature).ToList();
                var ranges = new List<ThresholdRange>();

                foreach (var threshold in thresholds)
                {
                    if (thresholds.FindIndex(p => p == threshold) == 0)
                        ranges.Add(new ThresholdRange(
                            start: int.MinValue,
                            end: threshold.Temperature,
                            value: 0));
                    else if (thresholds.FindIndex(p => p == threshold) == thresholds.Count() - 1)
                    {
                        ranges.Add(new ThresholdRange(
                            start: thresholds[thresholds.FindIndex(p => p == threshold) - 1].Temperature + 0.0001,
                            end: threshold.Temperature,
                            value: thresholds[thresholds.FindIndex(p => p == threshold) - 1].Value));

                        ranges.Add(new ThresholdRange(
                            start: threshold.Temperature,
                            end: int.MaxValue,
                            value: 1000));
                    }
                    else
                        ranges.Add(new ThresholdRange(
                            start: thresholds[thresholds.FindIndex(p => p == threshold) - 1].Temperature + 0.0001,
                            end: threshold.Temperature,
                            value: thresholds[thresholds.FindIndex(p => p == threshold) - 1].Value));
                }
                #endregion

                temperature = await _temperatureReader.GetTemperature();

                var value = ranges.SingleOrDefault(r => temperature >= r.Start && temperature <= r.End)?.Value;
                value = value ?? 0;

                if (previousValue != value)
                {
                    previousValue = value.Value;
                    _logger.LogInformation($"Updating PWM Temperature: {temperature}; Value: {value}");
                    await _processRunner.RunAsync(command: "gpio", new string[] { "pwm", _serviceConfigMonitor.CurrentValue.WiringPi.ToString(), value.ToString() });
                }

                Task.Delay(TimeSpan.FromSeconds(_serviceConfigMonitor.CurrentValue.TemperatureCheckIntervalSeconds)).Wait();
            }

            if (_serviceConfigMonitor.CurrentValue.ExitSound.Enabled)
                await _buzzer.Play(
                    frequency: 4000,
                    lenght: TimeSpan.FromSeconds(_serviceConfigMonitor.CurrentValue.StartSound.Interval))
                    .ConfigureAwait(false);

            await _processRunner.RunAsync(command: "gpio", new string[] { "pwm", _serviceConfigMonitor.CurrentValue.WiringPi.ToString(), _serviceConfigMonitor.CurrentValue.ValueToSetOnExit.ToString() });
        }
    }
}