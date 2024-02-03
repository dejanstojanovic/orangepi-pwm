using Iot.Device.Graphics;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Extensions;
using OrangePi.Display.Status.Service.Models;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class SsdInfoService : IInfoService
    {
        private readonly IProcessRunner _processRunner;
        private readonly ITemperatureReader _temperatureReader;
        private readonly string _driveMount;
        private readonly ILogger<SsdInfoService> _logger;
        public SsdInfoService(
            IProcessRunner processRunner,
            IEnumerable<ITemperatureReader> temperatureReaders,
            string driveMount,
            ILogger<SsdInfoService> logger)
        {
            _logger = logger;
            _processRunner = processRunner;
            _temperatureReader = temperatureReaders.Single(r => r.GetType() == typeof(SsdTemperatureReader));
            _driveMount = driveMount;
        }

        public string Label => "SSD";

        public async Task<BitmapImage> GetInfoDisplay(int screenWidth, int screenHeight, string fontName, int fontSize)
        {
            return await this.GetDisplay(screenWidth, screenHeight, fontName, fontSize);
        }

        public async Task<StatusValue> GetValue()
        {
            double fsUsage = 0;
            try
            {
                fsUsage = await _processRunner.RunAsync<double>("/bin/bash", $"-c \"df -H {_driveMount} --output=pcent | sed -e /Use%/d | grep -oP '(\\d+(\\.\\d+)?(?=%))'\"");
                fsUsage = Math.Round(fsUsage, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                fsUsage = 0;
            }

            double ssdTemp = 0;
            try
            {
                ssdTemp = await _temperatureReader.GetTemperature();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ssdTemp = 0;
            }

            return new StatusValue(
                valueText: $"{fsUsage.ToString("0.0")}%",
                value: fsUsage,
                note: $"{ssdTemp}°C");
        }
    }
}
