using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using SkiaSharp;
using System.Device.I2c;
using System.Drawing;

namespace OrangePi.Display.Status.Service
{

    public class Worker : BackgroundService
    {
        int screenWidth = 128;
        int screenHeight = 64;
        string fontName = "DejaVu Sans Bold";
        int fontSize = 12;
        int barHeight = 8;
        int spacing = 6;

        private readonly ILogger<Worker> _logger;
        private readonly ITemperatureService _temperatureService;
        private readonly IProcessRunner _processRunner;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly IGlancesService _glancesService;
        public Worker(
            ILogger<Worker> logger,
            ITemperatureService temperatureService,
            IProcessRunner processRunner,
            IOptions<ServiceConfiguration> serviceConfiguration,
            IGlancesService glancesService
            )
        {
            _logger = logger;
            _temperatureService = temperatureService;
            _processRunner = processRunner;
            _serviceConfiguration = serviceConfiguration.Value;
            _glancesService = glancesService;
            SkiaSharpAdapter.Register();
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var pause = _serviceConfiguration.IntervalTimeSpan;

            var values = new List<Func<Task<StatusValue>>>();

            #region Add values func
            values.Add(async () =>
            {
                try
                {
                    var cpuTemp = await _temperatureService.GetCpuTemperature();
                    return new StatusValue(
                        text:$"CPU temp {Math.Round(cpuTemp, 2)}°C", 
                        value:Math.Round(cpuTemp, 2));
                }
                catch
                {
                    return null;
                }
            });
            values.Add(async () =>
            {
                try
                {
                    var cpuUsage = await _glancesService.GetCpuUsage();
                    return new StatusValue(
                        text:$"CPU usage {Math.Round(cpuUsage.Total, 2)}%",
                        value: Math.Round(cpuUsage.Total, 2));
                }
                catch
                {
                    return null;
                }
            });
            values.Add(async () =>
            {
                try
                {
                    var memUsage = await _glancesService.GetMemoryUsage();
                    return new StatusValue(
                        text:$"RAM usage {Math.Round(memUsage.Percent, 2)}%",
                        value:Math.Round(memUsage.Percent, 2));
                }
                catch
                {
                    return null;
                }
            });

            #endregion


            //https://pinout.xyz/pinout/i2c
            using (var device = I2cDevice.Create(new I2cConnectionSettings(_serviceConfiguration.BusId, _serviceConfiguration.DeviceAddress)))
            {
                using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, screenWidth, screenHeight))
                {
                    if (_serviceConfiguration.Rotate)
                    {
                        ssd1306.SendCommand(new Ssd1306Command(0xc0));//Flip vertically
                        ssd1306.SendCommand(new Ssd1306Command(0xa0));//Flip horizontally
                    }

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        foreach (var value in values)
                        {
                            if (stoppingToken.IsCancellationRequested)
                                break;

                            await Task.Delay(pause);

                            if (_serviceConfiguration.BlinkOnChange)
                            {
                                ssd1306.ClearScreen();
                                await Task.Delay(TimeSpan.FromMilliseconds(200));
                            }

                            using (var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
                            {
                                image.Clear(Color.Black);
                                var g = image.GetDrawingApi();

                                int valuesDrawn = 0;
                                var c = g.GetCanvas();

                                var valueModel = await value();
                                if (valueModel != null)
                                {
                                    g.DrawText(text: valueModel.Text,
                                    fontFamilyName: fontName,
                                    size: fontSize,
                                    color: Color.White,
                                    position: new Point(0, valuesDrawn * (fontSize + spacing + barHeight)));

                                    valuesDrawn += 1;
                                    DrawBar(
                                    canvas: c,
                                    width: screenWidth,
                                    height: barHeight,
                                    startY: valuesDrawn * (fontSize + spacing) + ((valuesDrawn - 1) * barHeight),
                                    value: valueModel.Value);

                                }

                                ssd1306.DrawBitmap(image);
                            }


                        }
                    }
                    ssd1306.ClearScreen();
                }
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


    }
}