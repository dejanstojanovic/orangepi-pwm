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
        public CpuInfoService(
            IEnumerable<ITemperatureReader> temperatureReaders,
            IProcessRunner processRunner)
        {
            _temperatureReader = temperatureReaders.Single(r => r.GetType() == typeof(CpuTemperatureReader));
            _processRunner = processRunner;
        }

        public string Label => "CPU";

        public async Task<BitmapImage> GetInfoDisplay(int screenWidth, int screenHeight, string fontName, int fontSize)
        {
            return await this.GetDisplay(screenWidth,screenHeight,fontName, fontSize);
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
                catch
                {
                    cpuTemp = 0;
                }

                return cpuTemp;
            });

            var usageTask = Task.Run(async () =>
            {
                double cpuUsage = 0;
                try
                {
                    cpuUsage = await _processRunner.RunAsync<double>("grep 'cpu ' /proc/stat | awk '{usage=($2+$4)*100/($2+$4+$5)} END {print usage \"%\"}' | grep -oP \"(\\d+(\\.\\d+)?(?=%))\"");
                    cpuUsage = Math.Round(cpuUsage, 2);
                }
                catch { cpuUsage = 0; }
                return cpuUsage;
            });


            await Task.WhenAll<double>(tempTask, usageTask);

            return new StatusValue(
                valueText: $"{usageTask.Result}%",
                value: usageTask.Result,
                note: $"{tempTask.Result}°C");

        }
    }
}
