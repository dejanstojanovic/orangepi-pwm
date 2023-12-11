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
                double cpuTemp = 0;
                try
                {
                    cpuTemp = await _temperatureService.GetCpuTemperature();
                    cpuTemp = Math.Round(cpuTemp, 1);
                }
                catch
                {
                    cpuTemp = 0;
                }

                    return new StatusValue(
                        label:"CPU",
                        valueText:$"{cpuTemp}°C", 
                        value:cpuTemp);
                
            });
            values.Add(async () =>
            {
                double cpuUsage = 0;
                try
                {
                    var cpuUsageModel = await _glancesService.GetCpuUsage();
                    cpuUsage = Math.Round(cpuUsageModel.Total, 2);
                }
                catch { cpuUsage = 0; }
                return new StatusValue(
                    label: "CPU",
                    valueText: $"{cpuUsage}%",
                    value: cpuUsage);
                
            });
            values.Add(async () =>
            {
                double memUsage = 0;
                try
                {
                    var memUsageModel = await _glancesService.GetMemoryUsage();
                    memUsage = Math.Round(memUsageModel.Percent, 2);
                }
                catch { memUsage = 0; }

                return new StatusValue(
                    label: $"RAM",
                    valueText: $"{memUsage}%",
                    value: memUsage); 
             
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
                        foreach (var valueFunc in values)
                        {
                            if (stoppingToken.IsCancellationRequested)
                                break;

                            await Task.Delay(pause);

                            var value = await valueFunc();

                            if (_serviceConfiguration.BlinkOnChange)
                            {
                                ssd1306.ClearScreen();
                                await Task.Delay(TimeSpan.FromMilliseconds(200));
                            }
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
                                    SKRect sizeRect = new();
                                    valuePaint.MeasureText(value.ValueText, ref sizeRect);
                                    graphic.DrawText(text: value.ValueText,
                                        fontFamilyName: fontName,
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
                    }
                    ssd1306.ClearScreen();
                }
            }
        }

       


    }
}