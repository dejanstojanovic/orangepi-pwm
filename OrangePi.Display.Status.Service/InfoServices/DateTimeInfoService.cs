using Iot.Device.Graphics;
using OrangePi.Display.Status.Service.Extensions;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public class DateTimeInfoService : IDateTimeInfoService
    {
        public async Task<BitmapImage> GetInfoDisplay(int screenWidth, int screenHeight, string fontName, int fontSize)
        {
            return await this.GetDisplay(screenWidth, screenHeight, fontName, fontSize);
        }

        public async Task<DateTime> GetValue()
        {
            return DateTime.Now;
        }
    }
}
