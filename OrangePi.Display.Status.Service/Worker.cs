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
        private readonly ScreenConfiguration _screeConfiguration;
        private readonly SoundConfiguration _soundConfiguration;
        private readonly IProcessRunner _processRunner;
        readonly IEnumerable<IDisplayInfoService> _displayInfoServices;
        readonly string _currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        readonly ISwitch _switch;
        readonly System.Timers.Timer _playTimer;


        readonly object _lock = new object();
        bool _isPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                lock (_lock)
                {
                    _isPlaying = value;
                }
            }
        }
        public Worker(
            ILogger<Worker> logger,
            IOptions<ScreenConfiguration> serviceConfiguration,
            IOptions<SoundConfiguration> soundConfig,
            IEnumerable<IInfoService> infoServices,
            IProcessRunner processRunner,
            ISwitch @switch
            )
        {
            _logger = logger;
            _displayInfoServices = infoServices.Select(s => s as IDisplayInfoService).ToList();
            _screeConfiguration = serviceConfiguration.Value;
            _soundConfiguration = soundConfig.Value;
            _processRunner = processRunner;
            SkiaSharpAdapter.Register();
            _switch = @switch;
            _switch.Changed += _switch_Changed;

            IsPlaying = true;
            _playTimer = new System.Timers.Timer(_screeConfiguration.TimeOnTimeSpan)
            {
                Enabled = true,
                AutoReset = false
            };
            _playTimer.Elapsed += _playTimer_Elapsed;
        }

        private void _playTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            IsPlaying = false;
        }

        private void _switch_Changed(object? sender, bool isOn)
        {
            if (!isOn) return;

            if (!IsPlaying)
            {
                if (!string.IsNullOrWhiteSpace(_soundConfiguration.ActivationSound) && File.Exists(_soundConfiguration.ActivationSound))
                    _processRunner.Run(command: "play", _soundConfiguration.ActivationSound);
            }

            IsPlaying = true;
            _playTimer.Stop();
            _playTimer.Start();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!string.IsNullOrWhiteSpace(_soundConfiguration.ActivationSound) && File.Exists(_soundConfiguration.ActivationSound))
                _processRunner.Run(command: "play", _soundConfiguration.ActivationSound);

            var switchMonitor = _switch.StartMonitoringAsync(stoppingToken);
            var pause = _screeConfiguration.IntervalTimeSpan;

            //https://pinout.xyz/pinout/i2c
            using (var device = I2cDevice.Create(new I2cConnectionSettings(_screeConfiguration.BusId, _screeConfiguration.DeviceAddress)))
            {
                using (var ssd1306 = new Iot.Device.Ssd13xx.Ssd1306(device, screenWidth, screenHeight))
                {
                    if (_screeConfiguration.Rotate)
                    {
                        ssd1306.SendCommand(new Ssd1306Command(0xc0));//Flip vertically
                        ssd1306.SendCommand(new Ssd1306Command(0xa0));//Flip horizontally
                    }

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        if (!IsPlaying)
                        {
                            ssd1306.EnableDisplay(false);
                            await Task.Delay(TimeSpan.FromMilliseconds(100));
                            continue;
                        }

                        ssd1306.EnableDisplay(true);

                        foreach (var infoService in _displayInfoServices)
                        {
                            if (stoppingToken.IsCancellationRequested)
                                break;

                            using (var image = await infoService.GetInfoDisplay(screenWidth, screenHeight, fontName, fontSize))
                            {
                                ssd1306.DrawBitmap(image);
                            }
                            await Task.Delay(pause);
                        }

                    }

                    ssd1306.ClearScreen();
                }
            }

            await switchMonitor.WaitAsync(stoppingToken);
        }

    }
}