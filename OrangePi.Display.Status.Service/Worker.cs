using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using System.Device.I2c;
using System.Diagnostics;
using System.Drawing;

namespace OrangePi.Display.Status.Service
{

    public class Worker : BackgroundService
    {
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
            var fontSize = _serviceConfiguration.FontSize;
            var font = _serviceConfiguration.FontName;

            var values = new List<Func<Task<String>>>();
            values.Add(async () =>
            {
                var cpuTemp = await _temperatureService.GetCpuTemperature();
                return $"SoC:{Math.Round(cpuTemp, 1)}°C";
            });
            values.Add(async () =>
            {
                var cpuUsage = await _glancesService.GetCpuUsage();
                return $"CPU:{Math.Round(cpuUsage.Total, 1)}%";
            });
            values.Add(async () =>
            {
                var memUsage = await _glancesService.GetMemoryUsage();
                return $"MEM:{Math.Round(memUsage.Percent, 1)}%";
            });
            values.Add(async () => await Task.FromResult($"{DateTime.Now.ToString("hh:mm tt")}"));
            values.Add(async () => await Task.FromResult($"{DateTime.Now.ToString("yyyy-MM-dd")}"));

            var test = await _glancesService.GetCpuUsage();


            //https://pinout.xyz/pinout/i2c
            using (var device = I2cDevice.Create(new I2cConnectionSettings(_serviceConfiguration.BusId, _serviceConfiguration.DeviceAddress)))
            {
                using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, 128, 64))
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        foreach (var value in values)
                        {
                            using (var image = BitmapImage.CreateBitmap(ssd1306.ScreenWidth, ssd1306.ScreenHeight, PixelFormat.Format32bppArgb))
                            {
                                image.Clear(Color.Black);
                                var g = image.GetDrawingApi();

                                if (_serviceConfiguration.Rotate)
                                { 
                                    //TODO: investigate how to make this work
                                    var c = g.GetCanvas();
                                    c.Translate(image.Width / 2, image.Height / 2);
                                    c.RotateDegrees((float)180);
                                    c.Translate(-image.Width / 2, -image.Height / 2);
                                }

                                g.DrawText(text: await value(),
                                    fontFamilyName: font,
                                    size: fontSize,
                                    color: Color.White,
                                    position: new Point(_serviceConfiguration.OffsetX, _serviceConfiguration.OffsetY));

                                ssd1306.DrawBitmap(image);

                            }

                            if (stoppingToken.IsCancellationRequested)
                                break;

                            await Task.Delay(pause);

                            if (_serviceConfiguration.BlinkOnChange)
                            {
                                ssd1306.ClearScreen();
                                await Task.Delay(TimeSpan.FromMilliseconds(200));
                            }

                        }
                    }
                    ssd1306.ClearScreen();
                }
            }
        }


    }
}