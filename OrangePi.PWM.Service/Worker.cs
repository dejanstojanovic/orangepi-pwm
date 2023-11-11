using Microsoft.Extensions.Options;
using OrangePi.PWM.Service.Models;
using OrangePi.PWM.Service.Services;

namespace OrangePi.PWM.Service
{

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<ServiceConfiguration> _serviceConfigMonitor;
        private readonly IProcessRunner _processRunner;
        public Worker(
            ILogger<Worker> logger,
            IOptionsMonitor<ServiceConfiguration> serviceConfigMonitor,
            IProcessRunner processRunner)
        {
            _logger = logger;
            _serviceConfigMonitor = serviceConfigMonitor;
            _processRunner = processRunner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            double previousValue = 0;
            await _processRunner.RunAsync("gpio", "mode", _serviceConfigMonitor.CurrentValue.wPi.ToString(), "pwm");



            while (!stoppingToken.IsCancellationRequested)
            {
                var temeratureCheckOutput = await _processRunner.RunAsync("cat", "/sys/class/thermal/thermal_zone0/temp");

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

                if (double.TryParse(temeratureCheckOutput, out double temperature))
                {
                    if (temperature > 0)
                        temperature = temperature / 1000;

                    var value = ranges.SingleOrDefault(r => temperature >= r.Start && temperature <= r.End)?.Value;

                    value = value ?? 0;

                    if (previousValue != value)
                    {
                        previousValue = value.Value;
                        _logger.LogInformation($"Updating PWM Temperature: {temperature}; Value: {value}");

                        await _processRunner.RunAsync("gpio", "pwm", _serviceConfigMonitor.CurrentValue.wPi.ToString(), value.ToString());
                    }
                }
                else
                {
                    //Failed to read temperature, spin up fan to max
                    await _processRunner.RunAsync("gpio", "pwm", _serviceConfigMonitor.CurrentValue.wPi.ToString(), 1000.ToString());
                }

                Task.Delay(TimeSpan.FromSeconds(_serviceConfigMonitor.CurrentValue.IntervalSeconds)).Wait();
            }

            //Program exit, spin up fan to max
            await _processRunner.RunAsync("gpio", "pwm", _serviceConfigMonitor.CurrentValue.wPi.ToString(), 1000.ToString());
        }
    }
}