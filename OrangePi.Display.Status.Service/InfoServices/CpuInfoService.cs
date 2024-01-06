using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class CpuInfoService : IInfoService
    {
        private readonly ITemperatureReader _temperatureReader;
        private readonly IGlancesClient _glancesClient;
        public CpuInfoService(
            IEnumerable<ITemperatureReader> temperatureReaders,
            IGlancesClient glancesClient)
        {
            _temperatureReader = temperatureReaders.Single(r => r.GetType() == typeof(CpuTemperatureReader));
            _glancesClient = glancesClient;
        }

        public string Label => "CPU";

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
                    var cpuUsageModel = await _glancesClient.GetCpuUsage();
                    cpuUsage = Math.Round(cpuUsageModel.Total, 2);
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
