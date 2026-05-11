using Iot.Device.Graphics;

namespace OrangePi.Display.Status.Service.Services.Info
{
    public interface IDisplayInfoService
    {
        Task<BitmapImage> GetInfoDisplay(int screenWidth, int screenHeight, string fontName, int fontSize);
    }
}
