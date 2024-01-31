using Iot.Device.Graphics;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Extensions;
using OrangePi.Display.Status.Service.Models;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class RamInfoService : IInfoService
    {
        private readonly IProcessRunner _processRunner;
        public RamInfoService(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
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
                memUsage = await this._processRunner.RunAsync<double>("free -m | grep Mem | awk '{print ($3/$2)*100}'");
                memUsage = Math.Round(memUsage, 2);
                var usedGb = await this._processRunner.RunAsync<double>("free -m | grep Mem | awk '{print ($3/1000)}'");
                usedGbText = $"{Math.Round(usedGb, 2)} GB";
            }
            catch { memUsage = 0; }

            return new StatusValue(
                valueText: $"{memUsage}%",
                value: memUsage,
                note: usedGbText);
        }
    }
}
