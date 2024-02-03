using Iot.Device.Graphics;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Extensions;
using OrangePi.Display.Status.Service.Models;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class CpuInfoService : IInfoService
    {
        private readonly ITemperatureReader _temperatureReader;
        private readonly IProcessRunner _processRunner;
        private readonly ILogger<CpuInfoService> _logger;
        public CpuInfoService(
            IEnumerable<ITemperatureReader> temperatureReaders,
            IProcessRunner processRunner,
            ILogger<CpuInfoService> logger)
        {
            _temperatureReader = temperatureReaders.Single(r => r.GetType() == typeof(CpuTemperatureReader));
            _processRunner = processRunner;
            _logger = logger;

        }

        public string Label => "CPU";

        public async Task<BitmapImage> GetInfoDisplay(int screenWidth, int screenHeight, string fontName, int fontSize)
        {
            return await this.GetDisplay(screenWidth, screenHeight, fontName, fontSize);
        }

        public async Task<StatusValue> GetValue()
        {
            var tempTask = Task.Run(async () =>
            {
                double cpuTemp = 0;
                try
                {
                    cpuTemp = await _temperatureReader.GetTemperature();
                    cpuTemp = Math.Round(cpuTemp, 1);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    cpuTemp = 0;
                }

                return cpuTemp;
            });

            var usageTask = Task.Run(async () =>
            {
                double cpuUsage = 0;
                try
                {
                    var command = "-c \"grep 'cpu ' /proc/stat | awk '{usage=($2+$4)*100/($2+$4+$5)} END {print usage \"%\"}' | grep -oP '(\\d+(\\.\\d+)?(?=%))'\"";
                    var output = await _processRunner.RunAsync("/bin/bash", command);
                    cpuUsage = Math.Round(cpuUsage, 2);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    cpuUsage = 0;
                }
                return cpuUsage;
            });


            await Task.WhenAll<double>(tempTask, usageTask);

            return new StatusValue(
                valueText: $"{usageTask.Result.ToString("0.00")}%",
                value: usageTask.Result,
                note: $"{tempTask.Result}°C");

        }
    }
}
