using Iot.Device.CpuTemperature;
using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using OrangePi.Common.Services;
using System.Device.I2c;
using System.Drawing;
using UnitsNet;

namespace OrangePi.Display.Status.Service
{

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITemperatureService _temperatureService;
        private readonly IProcessRunner _processRunner;
        public Worker(
            ILogger<Worker> logger,
            ITemperatureService temperatureService,
            IProcessRunner processRunner
            )
        {
            _logger = logger;
            _temperatureService = temperatureService;
            _processRunner = processRunner;
            SkiaSharpAdapter.Register();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var fontSize = 16;
            var font = "DejaVu Sans";
            //var font = "Date Stamp";
            var y = 0;

            using (var device = I2cDevice.Create(new I2cConnectionSettings(5, 0x3c)))
            {
                using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, 128, 64))
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        using (var image = BitmapImage.CreateBitmap(128, 32, PixelFormat.Format32bppArgb))
                        {
                            var cpuTemp = Math.Round(await _temperatureService.GetCpuTemperature(),1);
                            //var cpuUsage = await _processRunner.RunAsync("echo", "\"$[100-$(vmstat 1 2|tail -1|awk '{print $15}')]\"");
                            image.Clear(Color.Black);
                            var g = image.GetDrawingApi();
                            g.DrawText($"CPU: {cpuTemp}°C", font, fontSize, Color.White, new Point(10, 5));
                            //g.DrawText($"CPU: {cpuTemp}°C", font, fontSize, Color.White, new Point(0, fontSize));
                            ssd1306.DrawBitmap(image);

                            await Task.Delay(1000);
                        }
                    }
                    ssd1306.ClearScreen();
                }
            }


        }


    }
}