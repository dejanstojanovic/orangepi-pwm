using Microsoft.Extensions.Options;
using OrangePi.PWM.Service.Models;
using System;
using System.Diagnostics;

namespace OrangePi.PWM.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<ServiceConfiguration> _serviceConfigMonitor;
        public Worker(
            ILogger<Worker> logger,
            IOptionsMonitor<ServiceConfiguration> serviceConfigMonitor)
        {
            _logger = logger;
            _serviceConfigMonitor = serviceConfigMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Process getProcess(string command, params string[] args)
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = command;
                foreach (var arg in args)
                {
                    process.StartInfo.ArgumentList.Add(arg);
                }
                return process;
            }

            int previousSpeed = 0;

            using (var pwmPinSet = getProcess("gpio", "mode", _serviceConfigMonitor.CurrentValue.wPi.ToString(), "pwm"))
            {
                pwmPinSet.Start();
                await pwmPinSet.WaitForExitAsync();
            }

            while (!stoppingToken.IsCancellationRequested)
            {

                using (var temeratureCheck = getProcess("cat", "/sys/class/thermal/thermal_zone0/temp"))
                {
                    temeratureCheck.Start();
                    string temeratureCheckOutput = temeratureCheck.StandardOutput.ReadToEnd();
                    await temeratureCheck.WaitForExitAsync();

                    if (int.TryParse(temeratureCheckOutput, out int temperature))
                    {
                        var speed = _serviceConfigMonitor.CurrentValue.TemperatureConfigurations.OrderBy(r => r.Temperature).Where(r => r.Temperature <= temperature).Last().Speed;
                        if (previousSpeed != speed)
                        {
                            previousSpeed = speed;

                            _logger.LogInformation($"Updating PWM Temperature: {temperature}; Speed: {speed}");

                            using (var pwmSet = getProcess("gpio", "pwm", _serviceConfigMonitor.CurrentValue.wPi.ToString(), speed.ToString()))
                            {
                                pwmSet.Start();
                                await pwmSet.WaitForExitAsync();
                            }
                        }
                    }
                }

                Task.Delay(TimeSpan.FromSeconds(_serviceConfigMonitor.CurrentValue.IntervalSeconds)).Wait();
            }
        }
    }
}