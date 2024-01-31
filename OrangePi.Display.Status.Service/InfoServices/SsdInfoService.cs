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
        public SsdInfoService(
            IProcessRunner processRunner,
            IEnumerable<ITemperatureReader> temperatureReaders, 
            string driveMount)
        {
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
                fsUsage = await _processRunner.RunAsync<double>($"df -H {_driveMount}--output=pcent | sed -e /Use%/d | grep -oP \"(\\d+(\\.\\d+)?(?=%))\"");
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
