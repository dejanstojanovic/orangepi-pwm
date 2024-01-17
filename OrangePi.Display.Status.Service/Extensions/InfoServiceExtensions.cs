using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.InfoServices;
using OrangePi.Display.Status.Service.Models;
using SkiaSharp;
using System.Device.Gpio;
using System.Device.I2c;
using System.Drawing;


namespace OrangePi.Display.Status.Service.Extensions
{
    public static class InfoServiceExtensions
    {
        public static async Task<BitmapImage> GetDonutChart(
            this IInfoService infoService,
            int screenWidth,
            int screenHeight,
            string fontName,
            int fontSize
            )
        {
            var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb);
            var value = await infoService.GetValue();
            image.Clear(Color.Black);
            var graphic = image.GetDrawingApi();
            var canvas = graphic.GetCanvas();

            //Draw border
            canvas.DrawArc(new SKRect(0, 0, screenHeight, screenHeight / 2), 0, 360, true, new SKPaint() { Color = SKColor.Parse("FFFFFF") });
            canvas.DrawArc(new SKRect(1, 1, screenHeight - 1, screenHeight / 2 - 1), 0, 360, true, new SKPaint() { Color = SKColor.Parse("000000") });

            //Draw value
            var angle = (int)Math.Round(((value.Value < 100 ? value.Value : 100) / 100) * 360);
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
                labelPaint.MeasureText(infoService.Label, ref sizeRect);
                graphic.DrawText(text: infoService.Label,
                    fontFamilyName: fontName,
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
                SKRect valueSizeRect = new();
                valuePaint.MeasureText(value.ValueText, ref valueSizeRect);
                graphic.DrawText(text: value.ValueText,
                    fontFamilyName: fontName,
                    size: (int)valuePaint.TextSize,
                    color: Color.White,
                    position: new Point(
                        x: (screenHeight + (int)(screenHeight - valueSizeRect.Width)) - 3,
                        y: (screenHeight / 4) - ((fontSize + 5) - 2) + 3)
                    );
                //Draw note
                if (!string.IsNullOrWhiteSpace(value.Note))
                {
                    using (var notePaint = new SKPaint
                    {
                        TextSize = 10,
                    })
                    {
                        SKRect noteSizeRect = new();
                        notePaint.MeasureText(value.Note, ref noteSizeRect);
                        graphic.DrawText(text: value.Note,
                            fontFamilyName: fontName,
                            size: 10,
                            color: Color.White,
                            position: new Point(
                                x: (screenHeight + (int)(screenHeight - noteSizeRect.Width)),
                                y: (screenHeight / 2) - fontSize)
                    );
                    }
                }
            }

            return image;

        }
    }
}
