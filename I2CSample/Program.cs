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
var fontSize = 12;
var barHeight = 8;
var spacing = 6;

//using (var device = I2cDevice.Create(new I2cConnectionSettings(5, 0x3c)))
//{
//    using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, screenWidth, screenHeight))
//    {

//        ssd1306.SendCommand(new Ssd1306Command(0xc0));//Flip vertically
//        ssd1306.SendCommand(new Ssd1306Command(0xa0));//Flip horizontally

//        ssd1306.SendCommand(new Ssd1306Command(0x81));
//        ssd1306.SendCommand(new Ssd1306Command(0x1F));

using (var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
{
    image.Clear(Color.Black);
    var g = image.GetDrawingApi();

    int valuesDrawn = 0;
    var c = g.GetCanvas();

    var values = new List<Func<Task<StatusValue>>>();
    values.Add(async () =>
    {
        return await Task.FromResult(new StatusValue("CPU usage 12.3%", 12.3));
    });

    c.DrawArc(new SKRect(0, 0, screenHeight, screenHeight), 0, 360, true, new SKPaint() { Color = SKColor.Parse("FFFFFF") });
    c.DrawArc(new SKRect(1, 1, screenHeight - 1, screenHeight - 1), 0, 360, true, new SKPaint() { Color = SKColor.Parse("000000") });

    c.DrawArc(new SKRect(3, 3, screenHeight - 3, screenHeight - 3), 0, 110, true, new SKPaint() { Color = SKColor.Parse("FFFFFF") });
    c.DrawArc(new SKRect(9, 9, screenHeight - 9, screenHeight - 9), 0, 360, true, new SKPaint() { Color = SKColor.Parse("000000") });


    c.DrawArc(new SKRect(11, 11, screenHeight - 11, screenHeight - 11), 0, 360, true, new SKPaint() { Color = SKColor.Parse("FFFFFF") });
    c.DrawArc(new SKRect(12, 12, screenHeight - 12, screenHeight - 12), 0, 360, true, new SKPaint() { Color = SKColor.Parse("000000") });

    var valueText = $"{77.9}°C";

    var paint = new SKPaint
    {
        TextSize = fontSize,
    };

    SKRect sizeRect = new();
    paint.MeasureText(valueText, ref sizeRect);

    g.DrawText(text: valueText,
        fontFamilyName: font,
        size: fontSize,
        color: Color.White,
        position: new Point(
            x: (screenHeight / 2) - ((int)sizeRect.Width / 2),
            y: (screenHeight / 2) - (fontSize - 2) + 2)
        );

    g.DrawText(text: "Temp.",
    fontFamilyName: font ,
    size: fontSize+ 3,
    color: Color.White,
    position: new Point(
        x: screenHeight + 10,
        y: (screenHeight / 2) - ((fontSize + 3) - 2) + 2)
    );

    //foreach (var value in values)
    //{
    //    var valueModel = await value();
    //    g.DrawText(text: valueModel.Text,
    //    fontFamilyName: font,
    //    size: fontSize,
    //    color: Color.White,
    //    position: new Point(0, valuesDrawn * (fontSize + spacing + barHeight)));

    //    valuesDrawn += 1;
    //    DrawBar(
    //    canvas: c,
    //    width: screenWidth,
    //    height: barHeight,
    //    startY: valuesDrawn * (fontSize + spacing) + ((valuesDrawn - 1) * barHeight),
    //    value: valueModel.Value);
    //}



    var path = @"d:\temp\status-display.png";
    if (File.Exists(path))
        File.Delete(path);
    image.SaveToFile(path, ImageFileType.Png);


    //    ssd1306.DrawBitmap(image);

    //}

    //Console.ReadKey();

    //ssd1306.ClearScreen();
    //}
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