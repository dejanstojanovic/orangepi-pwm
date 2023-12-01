using Iot.Device.CpuTemperature;
using Microsoft.Extensions.Options;
using OrangePi.PWM.Service.Models;
using OrangePi.PWM.Service.Services;
using UnitsNet;

namespace OrangePi.PWM.Service
{

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<ServiceConfiguration> _serviceConfigMonitor;
        private readonly IProcessRunner _processRunner;
        private readonly CpuTemperature _cpuTemperature;
        public Worker(
            ILogger<Worker> logger,
            IOptionsMonitor<ServiceConfiguration> serviceConfigMonitor,
            IProcessRunner processRunner,
            CpuTemperature cpuTemperature)
        {
            _logger = logger;
            _serviceConfigMonitor = serviceConfigMonitor;
            _processRunner = processRunner;
            _cpuTemperature = cpuTemperature;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var buzzer = new BuzzerService(_serviceConfigMonitor.CurrentValue.BuzzerPin))
            {
                await buzzer.Play(4600, TimeSpan.FromMicroseconds(300));
            }

            double previousValue = 0;
            await _processRunner.RunAsync("gpio", "mode", _serviceConfigMonitor.CurrentValue.wPi.ToString(), "pwm");

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

                if (!double.IsNaN(_cpuTemperature.Temperature.DegreesCelsius) &&
                    _cpuTemperature.Temperature.DegreesCelsius > 0)
                {
                    temperature = _cpuTemperature.Temperature.DegreesCelsius;
                }
                else
                {
                    var temeratureCheckOutput = await _processRunner.RunAsync("cat", "/sys/class/thermal/thermal_zone0/temp");
                    if (double.TryParse(temeratureCheckOutput, out double temperatureCheckValue) &&
                        temperatureCheckValue > 0)
                    {
                        temperature = temperatureCheckValue / 1000;
                    }
                }

                var value = ranges.SingleOrDefault(r => temperature >= r.Start && temperature <= r.End)?.Value;
                value = value ?? 0;

                if (previousValue != value)
                {
                    previousValue = value.Value;
                    _logger.LogInformation($"Updating PWM Temperature: {temperature}; Value: {value}");

                    await _processRunner.RunAsync("gpio", "pwm", _serviceConfigMonitor.CurrentValue.wPi.ToString(), value.ToString());
                }

                Task.Delay(TimeSpan.FromSeconds(_serviceConfigMonitor.CurrentValue.IntervalSeconds)).Wait();
            }

            //Program exit, set configured value
            using (var buzzer = new BuzzerService(_serviceConfigMonitor.CurrentValue.BuzzerPin))
            {
                await buzzer.Play(4600, TimeSpan.FromMicroseconds(600));
            }

            await _processRunner.RunAsync("gpio", "pwm", _serviceConfigMonitor.CurrentValue.wPi.ToString(), _serviceConfigMonitor.CurrentValue.ExitValue.ToString());
        }
    }
}