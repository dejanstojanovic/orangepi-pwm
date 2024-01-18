using Iot.Device.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public interface IDisplayInfoService
    {
        Task<BitmapImage> GetInfoDisplay(int screenWidth, int screenHeight, string fontName, int fontSize);
    }
}
