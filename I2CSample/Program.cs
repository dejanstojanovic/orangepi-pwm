using I2CSample;
using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Display.Status.Service.Models;
using SkiaSharp;
using System.Device.I2c;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

SkiaSharpAdapter.Register();

int screenWidth = 128;
int screenHeight = 64;
var font = "DejaVu Sans Bold";
var fontSize = 12;
var barHeight = 6;
var spacing = 3;

//using (var device = I2cDevice.Create(new I2cConnectionSettings(5, 0x3c)))
//{
//    using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, screenWidth, screenHeight))
//    {

//        ssd1306.SendCommand(new Ssd1306Command(0xc0));//Flip vertically
//        ssd1306.SendCommand(new Ssd1306Command(0xa0));//Flip horizontally

using (var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
{
    image.Clear(Color.Black);
    var g = image.GetDrawingApi();



    int valuesDrawn = 0;
    var c = g.GetCanvas();

    var values = new List<Func<Task<StatusValue>>>();
    values.Add(async () =>
    {
        return await Task.FromResult(new StatusValue("cpu usage 12.3%",12.3));
    });
    values.Add(async () =>
    {
        return await Task.FromResult(new StatusValue("cpu temp 32.7°C",32.7));
    });
    values.Add(async () =>
    {
        return await Task.FromResult(new StatusValue("ram usage 45.78%",45.78));
    });


    foreach(var value in values)
    {
        var valueModel = await value();
        g.DrawText(text: valueModel.Text,
        fontFamilyName: font,
        size: fontSize,
        color: Color.White,
        position: new Point(0, valuesDrawn * (fontSize + spacing + barHeight)));

        valuesDrawn += 1;
        DrawBar(
        canvas: c,
        width: screenWidth,
        height: barHeight,
        startY: valuesDrawn * (fontSize + spacing) + (valuesDrawn - 1) * barHeight,
        value: valueModel.Value);
    }

    //#region value 1
    //g.DrawText(text: "cpu usage 12.3%",
    //    fontFamilyName: font,
    //    size: fontSize,
    //    color: Color.White,
    //    position: new Point(0, valuesDrawn * (fontSize + spacing + barHeight)));

    //valuesDrawn += 1;
    //DrawBar(
    //canvas: c,
    //width: screenWidth,
    //height: barHeight,
    //startY: valuesDrawn * (fontSize + spacing) + (valuesDrawn - 1) * barHeight,
    //value: 12.786);
    //#endregion

    //#region value 2
    //g.DrawText(text: "cpu usage 12.3%",
    //    fontFamilyName: font,
    //    size: fontSize,
    //    color: Color.White,
    //    position: new Point(0, valuesDrawn * (fontSize + spacing + barHeight)));
    //valuesDrawn += 1;
    //DrawBar(
    //    canvas:c, 
    //    width: screenWidth, 
    //    height: barHeight,
    //    startY: valuesDrawn * (fontSize + spacing) + (valuesDrawn - 1) * barHeight, 
    //    value:12.786);
    //#endregion

    //#region value 3
    //g.DrawText(text: "cpu usage 12.3%",
    //    fontFamilyName: font,
    //    size: fontSize,
    //    color: Color.White,
    //    position: new Point(0, valuesDrawn * (fontSize + spacing + barHeight)));
    //valuesDrawn += 1;
    //DrawBar(
    //canvas: c,
    //width: screenWidth,
    //height: barHeight,
    //startY: valuesDrawn * (fontSize + spacing) + (valuesDrawn - 1) * barHeight,
    //value: 12.786);
    //#endregion

    var path = @"d:\temp\status-display.png";
    if (File.Exists(path))
        File.Delete(path);
    image.SaveToFile(path, ImageFileType.Png);


    //ssd1306.DrawBitmap(image);

    //}

    //Console.ReadKey();

    //ssd1306.ClearScreen();
    //}
}


void DrawBar(SKCanvas canvas, int width, int height, int startY, double value)
{
    canvas.DrawRect(0, startY, width, 6, new SKPaint()
    {
        Color = SKColor.Parse("FFFFFF"),
    });
    canvas.DrawRect(1, startY + 1, width - 2, 4, new SKPaint()
    {
        Color = SKColor.Parse("000000"),
    });

    //TODO: calculate value here
    var maxWidth = width - 2;
    var valueWidth = (int)Math.Round((value / maxWidth) * 100);

    canvas.DrawRect(2, startY + 2, valueWidth, 2, new SKPaint()
    {
        Color = SKColor.Parse("FFFFFF"),
    });
}