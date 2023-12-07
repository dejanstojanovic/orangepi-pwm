using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Display.Status.Service.Models;
using System.Device.I2c;
using System.Diagnostics;
using System.Drawing;

SkiaSharpAdapter.Register();
using (var device = I2cDevice.Create(new I2cConnectionSettings(5, 0x3c)))
{
    using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, 128, 64))
    {

        ssd1306.SendCommand(new Ssd1306Command(0xc0));//Flip vertically
        ssd1306.SendCommand(new Ssd1306Command(0xa0));//Flip horizontally

        using (var image = BitmapImage.CreateBitmap(ssd1306.ScreenWidth, ssd1306.ScreenHeight, PixelFormat.Format32bppArgb))
        {
            image.Clear(Color.Black);
            var g = image.GetDrawingApi();

            var font = "DejaVu Sans Bold";
            var fontSize = 12;

            g.DrawText(text: "CPU 12.3% 32.8°C",
                fontFamilyName: font,
                size: fontSize,
                color: Color.White,
                position: new Point(0, 0));

            g.DrawText(text: "RAM 32.8% 12.8GB",
                fontFamilyName: font,
                size: fontSize,
                color: Color.White,
                position: new Point(0, fontSize));

            //g.DrawText(text: "IO 23.6% 120.2/512GB",
            //   fontFamilyName: font,
            //   size: fontSize,
            //   color: Color.White,
            //   position: new Point(0, (fontSize * 2)));

            //g.DrawText(text: "UP 123 days, 20:30:06",
            //   fontFamilyName: font,
            //   size: fontSize,
            //   color: Color.White,
            //   position: new Point(0, fontSize));

            ssd1306.DrawBitmap(image);

        }

        Console.ReadKey();

        ssd1306.ClearScreen();
    }
}
