using Iot.Device.Graphics;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Extensions;
using OrangePi.Display.Status.Service.Models;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class RamInfoService : IInfoService
    {
        private readonly IGlancesClient _glancesService;
        public RamInfoService(IGlancesClient glancesService)
        {
            _glancesService = glancesService;
        }

        public string Label => "RAM";

        public async Task<BitmapImage> GetInfoDisplay(int screenWidth, int screenHeight, string fontName, int fontSize)
        {
            return await this.GetDisplay(screenWidth, screenHeight, fontName, fontSize);
        }

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
