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
var fontSize = 12;

using (var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
{
    image.Clear(Color.Black);
    var graphic = image.GetDrawingApi();
    var canvas = graphic.GetCanvas();

    var time = DateTime.Now.ToString("HH:mm");
    var date = DateTime.Now.ToString("yyyy-MM-dd");
    //Draw Time
    var timeFontSize = fontSize + 8;
    using (var timeLabelPaint = new SKPaint
    {
        TextSize = timeFontSize,
    })
    {
        SKRect timeSizeRect = new();
        timeLabelPaint.MeasureText(time, ref timeSizeRect);

        graphic.DrawText(text: time,
            fontFamilyName: font,
            size: timeFontSize,
            color: Color.White,
            position: new Point(
                x: (screenWidth / 2) - ((int)timeSizeRect.Width / 2),
                y: ((screenHeight / 4) - ((int)timeSizeRect.Height * 2) / 2) - 4
            ));

        using (var dateLabelPaint = new SKPaint
        {
            TextSize = fontSize,
        })
        {
            SKRect dateSizeRect = new();
            dateLabelPaint.MeasureText(date, ref dateSizeRect);

            graphic.DrawText(text: date,
                fontFamilyName: font,
                size: fontSize,
                color: Color.White,
                position: new Point(
                    x: (screenWidth / 2) - ((int)dateSizeRect.Width / 2),
                    y: ((screenHeight / 4) - ((int)timeSizeRect.Height * 2) / 2) + (int)timeSizeRect.Height + 2
                ));
        }
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
