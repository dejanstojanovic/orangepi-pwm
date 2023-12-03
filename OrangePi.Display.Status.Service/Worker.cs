using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using System.Device.I2c;
using System.Drawing;

namespace OrangePi.Display.Status.Service
{

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITemperatureService _temperatureService;
        private readonly IProcessRunner _processRunner;
        private readonly ServiceConfiguration _serviceConfiguration;
        public Worker(
            ILogger<Worker> logger,
            ITemperatureService temperatureService,
            IProcessRunner processRunner,
            IOptions<ServiceConfiguration> serviceConfiguration
            )
        {
            _logger = logger;
            _temperatureService = temperatureService;
            _processRunner = processRunner;
            _serviceConfiguration = serviceConfiguration.Value;
            SkiaSharpAdapter.Register();
        }

        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var pause = _serviceConfiguration.Pause;
            var fontSize = 16;
            var font = "DejaVu Sans";
            //var font = "Date Stamp";
            var y = 0;

            var values = new List<Func<Task<String>>>();
            values.Add(async () =>
            {
                var cpuTemp = await _temperatureService.GetCpuTemperature();
                return $"CPU: {Math.Round(cpuTemp, 1)}°C";
            });
            //values.Add(async () => await Task.FromResult("val 2"));

            //https://pinout.xyz/pinout/i2c
            using (var device = I2cDevice.Create(new I2cConnectionSettings(_serviceConfiguration.BusId, _serviceConfiguration.DeviceAddress)))
            {
                using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, 128, 64))
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        foreach (var value in values)
                        {
                            using (var image = BitmapImage.CreateBitmap(128, 32, PixelFormat.Format32bppArgb))
                            {
                                image.Clear(Color.Black);
                                var g = image.GetDrawingApi();
                                g.DrawText(await value(), font, fontSize, Color.White, new Point(10, 5));
                                ssd1306.DrawBitmap(image);

                            }
                            await Task.Delay(pause);
                        }
                    }
                    ssd1306.ClearScreen();
                }
            }
        }


    }
}