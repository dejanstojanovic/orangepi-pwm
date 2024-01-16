using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using SkiaSharp;
using System.Drawing;
using UnitsNet;

SkiaSharpAdapter.Register();

int screenWidth = 128;
int screenHeight = 64;
var font = "DejaVu Sans Bold";
var ipFontSize = 18;

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
                y: ((screenHeight / 2) - ((int)ipSizeRect.Height * 2) / 2) - 10
            ));

        SKRect hostSizeRect = new();
        labelPaint.MeasureText(host, ref hostSizeRect);

        graphic.DrawText(text: host,
            fontFamilyName: font,
            size: ipFontSize,
            color: Color.White,
            position: new Point(
                x: (screenWidth / 2) - ((int)hostSizeRect.Width / 2),
                y: ((screenHeight / 2) - ((int)hostSizeRect.Height * 2) / 2) + (int)hostSizeRect.Height 
            ));
    }

    var filePath = @"C:\temp\ip-info.png";
    if (File.Exists(filePath))
        File.Delete(filePath);
    image.SaveToFile(filePath, ImageFileType.Png);

}