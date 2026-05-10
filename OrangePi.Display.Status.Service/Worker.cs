using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using OrangePi.Display.Status.Service.Services.Info;
using OrangePi.Display.Status.Service.Services.Switch;
using System.Device.I2c;
using System.Reflection;

namespace OrangePi.Display.Status.Service
{
    public class Worker : BackgroundService
    {

        readonly int screenWidth = 128;
        readonly int screenHeight = 64;
        readonly string fontName = "DejaVu Sans Bold";
        readonly int fontSize = 12;

        private readonly ILogger<Worker> _logger;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly SoundConfiguration _soundConfiguration;
        private readonly IProcessRunner _processRunner;
        readonly IEnumerable<IDisplayInfoService> _displayInfoServices;
        readonly string _currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        readonly ISwitch _switch;
        public Worker(
            ILogger<Worker> logger,
            IOptions<ServiceConfiguration> serviceConfiguration,
            IOptions<SoundConfiguration> soundConfig,
            IEnumerable<IInfoService> infoServices,
            IProcessRunner processRunner,
            ISwitch @switch
            )
        {
            _logger = logger;
            _displayInfoServices = infoServices.Select(s => s as IDisplayInfoService).ToList();
            _serviceConfiguration = serviceConfiguration.Value;
            _soundConfiguration = soundConfig.Value;
            _processRunner = processRunner;
            SkiaSharpAdapter.Register();
            _switch = @switch;
            _switch.Changed += _switch_Changed;
        }

        private void _switch_Changed(object? sender, bool e)
        {
            if (!e && !string.IsNullOrWhiteSpace(_soundConfiguration.ActivationSound) && File.Exists(_soundConfiguration.ActivationSound))
                _processRunner.Run(command: "play", _soundConfiguration.ActivationSound);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!string.IsNullOrWhiteSpace(_soundConfiguration.ActivationSound) && File.Exists(_soundConfiguration.ActivationSound))
                _processRunner.Run(command: "play", _soundConfiguration.ActivationSound);

            var switchMonitor = _switch.StartMonitoringAsync(stoppingToken);
            var pause = _serviceConfiguration.IntervalTimeSpan;

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
                        if (!_switch.IsOn)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(100));
                            ssd1306.EnableDisplay(false);
                            continue;
                        }

                        ssd1306.EnableDisplay(true);

                        foreach (var infoService in _displayInfoServices)
                        {
                            if (stoppingToken.IsCancellationRequested)
                                break;

                            await Task.Delay(pause);

                            using (var image = await infoService.GetInfoDisplay(screenWidth, screenHeight, fontName, fontSize))
                            {
                                ssd1306.DrawBitmap(image);
                            }

                        }
                    }
                    ssd1306.ClearScreen();
                }
            }

            await switchMonitor.WaitAsync(stoppingToken);
        }

    }
}