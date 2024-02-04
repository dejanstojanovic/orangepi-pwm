using Iot.Device.Graphics;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Extensions;
using OrangePi.Display.Status.Service.Models;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class RamInfoService : IInfoService
    {
        private readonly IProcessRunner _processRunner;
        private readonly ILogger<RamInfoService> _logger;
        public RamInfoService(IProcessRunner processRunner, ILogger<RamInfoService> logger)
        {
            _processRunner = processRunner;
            _logger = logger;

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
                memUsage = await this._processRunner.RunAsync<double>("/bin/bash", "-c \"free -m | grep Mem | awk '{print ($3/$2)*100}'\"");
                memUsage = Math.Round(memUsage, 1);
                var usedGb = await this._processRunner.RunAsync<double>("/bin/bash", " -c \"free -m | grep Mem | awk '{print ($3/1000)}'\"");
                usedGbText = $"{Math.Round(usedGb, 2)} GB";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                memUsage = 0;
            }

            return new StatusValue(
                valueText: $"{memUsage.ToString("0.0")}%",
                value: memUsage,
                note: usedGbText);
        }
    }
}
