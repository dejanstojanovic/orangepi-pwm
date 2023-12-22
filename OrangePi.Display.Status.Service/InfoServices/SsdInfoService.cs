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
        private readonly IGlancesService _glancesService;
        public SsdInfoService(IGlancesService glancesService)
        {
            _glancesService = glancesService;
        }

        public string Label => "SSD";

        public async Task<StatusValue> GetValue()
        {
            double fsUsage = 0;
            string? usedGbText = null;
            try
            {
                var fsUsageModel = await _glancesService.GetFileSystemUsage("/etc/hostname");
                fsUsage = Math.Round(fsUsageModel.Percent, 2);
                usedGbText = $"{Math.Round((fsUsageModel.Used * 1.00) / 1000000000, 2)} GB";
            }
            catch { fsUsage = 0; }
            return new StatusValue(
                valueText: $"{fsUsage}%",
                value: fsUsage,
                note: usedGbText);
        }
    }
}
