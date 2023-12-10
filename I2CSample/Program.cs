using I2CSample;
using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Iot.Device.Mcp25xxx.Register.ErrorDetection;
using Microsoft.Extensions.Options;
using OrangePi.Display.Status.Service.Models;
using SkiaSharp;
using System.Device.I2c;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

bool active = false;

SkiaSharpAdapter.Register();

int screenWidth = 128;
int screenHeight = 64;
var font = "DejaVu Sans Bold";
var fontSize = 10;
var barHeight = 8;
var spacing = 6;

var values = new List<Func<Task<StatusValue>>>();
values.Add(async () =>
{
    var value = 36.9;
    return await Task.FromResult(new StatusValue("SOC", value, $"{value}°C"));
});

using (var device = I2cDevice.Create(new I2cConnectionSettings(5, 0x3c)))
{
    using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, screenWidth, screenHeight))
    {

        ssd1306.SendCommand(new Ssd1306Command(0xc0));//Flip vertically
        ssd1306.SendCommand(new Ssd1306Command(0xa0));//Flip horizontally

        foreach (var valueFunc in values)
        {
            var value = await valueFunc();
            using (var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
            {
                image.Clear(Color.Black);
                var graphic = image.GetDrawingApi();
                var canvas = graphic.GetCanvas();

                //Draw border
                canvas.DrawArc(new SKRect(0, 0, screenHeight, screenHeight / 2), 0, 360, true, new SKPaint() { Color = SKColor.Parse("FFFFFF") });
                canvas.DrawArc(new SKRect(1, 1, screenHeight - 1, screenHeight / 2 - 1), 0, 360, true, new SKPaint() { Color = SKColor.Parse("000000") });

                //Draw value
                var angle = (int)Math.Round((value.Value / 100) * 360);
                canvas.DrawArc(new SKRect(1, 1, screenHeight - 1, screenHeight / 2 - 1), 0, angle, true, new SKPaint() { Color = SKColor.Parse("FFFFFF") });

                //Draw inner circle
                canvas.DrawArc(new SKRect(8, 4, screenHeight - 8, (screenHeight / 2) - 4), 0, 360, true, new SKPaint() { Color = SKColor.Parse("FFFFFF") });
                canvas.DrawArc(new SKRect(10, 5, screenHeight - 10, screenHeight / 2 - 5), 0, 360, true, new SKPaint() { Color = SKColor.Parse("000000") });

                //Draw label
                using (var labelPaint = new SKPaint
                {
                    TextSize = fontSize,
                })
                {
                    SKRect sizeRect = new();
                    labelPaint.MeasureText(value.Label, ref sizeRect);
                    graphic.DrawText(text: value.Label,
                        fontFamilyName: font,
                        size: fontSize,
                        color: Color.White,
                        position: new Point(
                            x: (screenHeight / 2) - ((int)sizeRect.Width / 2),
                            y: (screenHeight / 4) - (fontSize - 2) + 2)
                        );
                }

                //Draw value
                using (var valuePaint = new SKPaint
                {
                    TextSize = fontSize + 5,
                })
                {
                    SKRect sizeRect = new();
                    valuePaint.MeasureText(value.ValueText, ref sizeRect);
                    graphic.DrawText(text: value.ValueText,
                        fontFamilyName: font,
                        size: (int)valuePaint.TextSize,
                        color: Color.White,
                        position: new Point(
                            x: screenHeight + (int)(screenHeight - sizeRect.Width) / 2,
                            y: (screenHeight / 4) - ((fontSize + 5) - 2) + 3
                            )
                        );
                }


                ssd1306.DrawBitmap(image);
            }
        }

        Console.ReadKey();

        ssd1306.ClearScreen();
    }
}


void DrawBar(SKCanvas canvas, int width, int height, int startY, double value)
{
    canvas.DrawRect(1, startY, width - 1, height, new SKPaint()
    {
        Color = SKColor.Parse("FFFFFF"),
    });
    canvas.DrawRect(2, startY + 1, width - 3, height - 2, new SKPaint()
    {
        Color = SKColor.Parse("000000"),
    });

    //TODO: calculate value here
    var maxWidth = width - 4;
    var valueWidth = (int)Math.Round((value / maxWidth) * 100);

    canvas.DrawRect(3, startY + 2, valueWidth, height - 4, new SKPaint()
    {
        Color = SKColor.Parse("FFFFFF"),
    });
}

void StartMonitorActive()
{
    Task.Run(async () =>
    {
        while (true)//TODO: Use cancellation token here
        {
            active = true;
            await Task.Delay(TimeSpan.FromMilliseconds(10));
        }
    });

}