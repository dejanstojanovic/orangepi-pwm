using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class SsdInfoService : IInfoService
    {
        private readonly IGlancesClient _glancesService;
        private readonly ITemperatureReader _temperatureReader;
        public SsdInfoService(
            IGlancesClient glancesService,
            IEnumerable<ITemperatureReader> temperatureReaders)
        {
            _glancesService = glancesService;
            _temperatureReader = temperatureReaders.Single(r => r.GetType() == typeof(SsdTemperatureReader));

        }

        public string Label => "SSD";

        public async Task<StatusValue> GetValue()
        {
            double fsUsage = 0;
            try
            {
                var fsUsageModel = await _glancesService.GetFileSystemUsage("/etc/hostname");
                fsUsage = Math.Round(fsUsageModel.Percent, 2);
            }
            catch { fsUsage = 0; }

            double ssdTemp = 0;
            try
            {
                ssdTemp = await _temperatureReader.GetTemperature();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERR: {ex.Message}");
                ssdTemp = 0;
            }


            return new StatusValue(
                valueText: $"{fsUsage}%",
                value: fsUsage,
                note: $"{ssdTemp}°C");
        }
    }
}
