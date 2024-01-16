using DrawingSample;
using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using SkiaSharp;
using System.Device.I2c;
using System.Drawing;
using UnitsNet;


SkiaSharpAdapter.Register();

int screenWidth = 128;
int screenHeight = 64;
var font = "DejaVu Sans Bold";
var ipFontSize = 12;

using (var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
{
    image.Clear(Color.Black);
    var graphic = image.GetDrawingApi();
    var canvas = graphic.GetCanvas();

    var ip = "192.168.1.25";
    var host = "orangepi5";
    //Draw IP
    using (var labelPaint = new SKPaint
    {
        TextSize = ipFontSize,
    })
    {
        SKRect ipSizeRect = new();
        labelPaint.MeasureText(ip, ref ipSizeRect);

        graphic.DrawText(text: ip,
            fontFamilyName: font,
            size: ipFontSize,
            color: Color.White,
            position: new Point(
                x: (screenWidth / 2) - ((int)ipSizeRect.Width / 2),
                y: ((screenHeight / 4) - ((int)ipSizeRect.Height * 2) / 2) -2
            ));

        SKRect hostSizeRect = new();
        labelPaint.MeasureText(host, ref hostSizeRect);

        graphic.DrawText(text: host,
            fontFamilyName: font,
            size: ipFontSize,
            color: Color.White,
            position: new Point(
                x: (screenWidth / 2) - ((int)hostSizeRect.Width / 2),
                y: ((screenHeight / 4) - ((int)hostSizeRect.Height * 2) / 2) + (int)hostSizeRect.Height
            ));
    }

    var useDisplay = true;

    if (!useDisplay)
    {
        var filePath = @"C:\temp\ip-info.png";
        if (File.Exists(filePath))
            File.Delete(filePath);
        image.SaveToFile(filePath, ImageFileType.Png);
    }
    else
    {
        var busId = 5;
        var deviceAddress = 60;
        using (var device = I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress)))
        {
            using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, screenWidth, screenHeight))
            {
                ssd1306.SendCommand(new Ssd1306Command(0xc0));//Flip vertically
                ssd1306.SendCommand(new Ssd1306Command(0xa0));//Flip horizontally

                ssd1306.ClearScreen();
                ssd1306.DrawBitmap(image);
            }
        }
    }

}