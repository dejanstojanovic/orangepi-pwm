using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class RamInfoService : IInfoService
    {
        private readonly IGlancesService _glancesService;
        public RamInfoService(IGlancesService glancesService)
        {
            _glancesService = glancesService;
        }

        public string Label => "RAM";

        public async Task<StatusValue> GetValue()
        {
            double memUsage = 0;
            string? usedGbText = null;
            try
            {
                var memUsageModel = await _glancesService.GetMemoryUsage();
                memUsage = Math.Round(memUsageModel.Percent, 2);
                usedGbText = $"{Math.Round((memUsageModel.Used * 1.00) / 1000000000, 2)} GB";
            }
            catch { memUsage = 0; }

            return new StatusValue(
                valueText: $"{memUsage}%",
                value: memUsage,
                note: usedGbText);
        }
    }
}
