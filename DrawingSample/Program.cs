using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using SkiaSharp;
using System.Drawing;
using UnitsNet;

SkiaSharpAdapter.Register();

int screenWidth = 128;
int screenHeight = 64;
var font = "DejaVu Sans Bold";
var ipFontSize = 20;

using (var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
{
    image.Clear(Color.Black);
    var graphic = image.GetDrawingApi();
    var canvas = graphic.GetCanvas();

    var ip = "255.255.255.255";
    //Draw IP
    using (var ipLabelPaint = new SKPaint
    {
        TextSize = ipFontSize,
    })
    {
        SKRect sizeRect = new();
        ipLabelPaint.MeasureText(ip, ref sizeRect);
        graphic.DrawText(text: ip,
            fontFamilyName: font,
            size: ipFontSize,
            color: Color.White,
            position: new Point(
                x: (screenHeight / 2) - ((int)sizeRect.Width / 2),
                y: 0 //(screenHeight / 4) - (ipFontSize - 2) + 2)
            ));
    }

    var filePath = @"C:\temp\ip-info.png";
    if(File.Exists(filePath))
        File.Delete(filePath);
    image.SaveToFile(filePath , ImageFileType.Png);

}