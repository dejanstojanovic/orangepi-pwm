using Iot.Device.Graphics;
using Iot.Device.Ssd13xx;
using OrangePi.Display.Status.Service.Models;
using System.Device.I2c;

namespace OrangePi.Display.Status.Service.Services
{
    public class Screen : IScreen
    {
        public int Width => 128;

        public int Height => 64;

        readonly I2cDevice _i2CDevice;
        readonly Ssd1306 _ssd1306;
        public Screen(int busId, int deviceAddress)
        {
            _i2CDevice = I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress));
            _ssd1306 = new Ssd1306(_i2CDevice, Width, Height);
        }

        public void Clear()
        {
            _ssd1306.ClearScreen();
        }

        public void Disable()
        {
            _ssd1306.EnableDisplay(false);
        }

        public void DrawImage(BitmapImage bitmapImage)
        {
            _ssd1306.DrawBitmap(bitmapImage);
        }

        public void Enable()
        {
            _ssd1306.EnableDisplay(true);
        }

        public void Flip(IEnumerable<IScreen.FlipType> rotationTypes)
        {
            if (rotationTypes.Contains(IScreen.FlipType.Horizontally))
                FlipHorizontally();
            if (rotationTypes.Contains(IScreen.FlipType.Vertically))
                FlipVertically();
        }

        public void FlipHorizontally()
        {
            _ssd1306.SendCommand(new Ssd1306Command(0xc0));
        }

        public void FlipVertically()
        {
            _ssd1306.SendCommand(new Ssd1306Command(0xc0));
        }
        public void Dispose()
        {
            _ssd1306.Dispose();
            _i2CDevice.Dispose();
        }


    }
}
