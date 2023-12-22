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
        private readonly ITemperatureService _temperatureService;
        private readonly IGlancesService _glancesService;
        public CpuInfoService(
            ITemperatureService temperatureService,
            IGlancesService glancesService)
        {
            _temperatureService = temperatureService;
            _glancesService = glancesService;
        }

        public string Label => "CPU";

        public async Task<StatusValue> GetValue()
        {
            double cpuTemp = 0;
            try
            {
                cpuTemp = await _temperatureService.GetCpuTemperature();
                 cpuTemp = Math.Round(cpuTemp, 1);
            }
            catch
            {
                cpuTemp = 0;
            }

            double cpuUsage

            return new StatusValue(
                label: "CPU",
                valueText: $"{cpuTemp}°C",
                value: cpuTemp,
                note: $"{}°%");

        }
    }
}
