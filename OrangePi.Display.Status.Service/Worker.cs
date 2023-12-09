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

            #region Add values func
            values.Add(async () =>
            {
                try
                {
                    var cpuTemp = await _temperatureService.GetCpuTemperature();
                    return $"T:{Math.Round(cpuTemp, 2)}°C";
                }
                catch
                {
                    return "T: ?";
                }
            });
            values.Add(async () =>
            {
                try
                {
                    var cpuUsage = await _glancesService.GetCpuUsage();
                    return $"CPU:{Math.Round(cpuUsage.Total, 1)}%";
                }
                catch
                {
                    return "CPU: ?";
                }
            });
            values.Add(async () =>
            {
                try
                {
                    var memUsage = await _glancesService.GetMemoryUsage();
                    return $"RAM:{Math.Round(memUsage.Percent, 1)}%";
                }
                catch
                {
                    return "RAM: ?";
                }
            });
            values.Add(async () => await Task.FromResult($"{DateTime.Now.ToString("hh:mm tt")}"));
            values.Add(async () => await Task.FromResult($"{DateTime.Now.ToString("yyyy-MM-dd")}"));
            #endregion


            //https://pinout.xyz/pinout/i2c
            using (var device = I2cDevice.Create(new I2cConnectionSettings(_serviceConfiguration.BusId, _serviceConfiguration.DeviceAddress)))
            {
                using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, 128, 64))
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
                            using (var image = BitmapImage.CreateBitmap(ssd1306.ScreenWidth, ssd1306.ScreenHeight, PixelFormat.Format32bppArgb))
                            {
                                image.Clear(Color.Black);
                                var g = image.GetDrawingApi();



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