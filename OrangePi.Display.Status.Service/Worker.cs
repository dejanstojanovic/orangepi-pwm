using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.InfoServices;
using OrangePi.Display.Status.Service.Models;
using System.Device.Gpio;
using System.Device.I2c;
using System.Reflection;
using UnitsNet;

namespace OrangePi.Display.Status.Service
{
    public class Worker : BackgroundService
    {
        #region Switch mechanism
        bool _switch = true;
        readonly object _lock = new Object();
        public bool Switch
        {
            get
            {
                lock (_lock)
                {
                    return _switch;
                }
            }
            set
            {
                lock (_lock)
                {
                    _switch = value;
                    if (value)
                    {
                        _timer.Stop();
                        _timer.Start();
                    }
                }
            }
        }
        #endregion

        readonly int screenWidth = 128;
        readonly int screenHeight = 64;
        readonly string fontName = "DejaVu Sans Bold";
        readonly int fontSize = 12;
        readonly int volume = 80;

        private readonly ILogger<Worker> _logger;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly SwitchConfig _switchConfig;
        private readonly SoundConfiguration _soundConfiguration;
        private readonly IProcessRunner _processRunner;
        readonly System.Timers.Timer _timer;
        readonly IEnumerable<IDisplayInfoService> _displayInfoServices;
        readonly string _currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public Worker(
            ILogger<Worker> logger,
            IOptions<ServiceConfiguration> serviceConfiguration,
            IOptions<SwitchConfig> switchConfig,
            IOptions<SoundConfiguration> soundConfig,
            IEnumerable<IInfoService> infoServices,
            IProcessRunner processRunner
            //IHostInfoService hostInfoService,
            //IDateTimeInfoService dateTimeInfoService
            )
        {
            _logger = logger;
            _displayInfoServices = infoServices.Select(s => s as IDisplayInfoService).ToList();

            //_displayInfoServices = _displayInfoServices.Prepend(hostInfoService);
            //_displayInfoServices = _displayInfoServices.Prepend(dateTimeInfoService);

            _serviceConfiguration = serviceConfiguration.Value;
            _switchConfig = switchConfig.Value;
            _soundConfiguration = soundConfig.Value;
            _processRunner = processRunner;
            _timer = new System.Timers.Timer(_serviceConfiguration.TimeOnTimeSpan);
            _timer.Elapsed += timer_Elapsed;
            SkiaSharpAdapter.Register();
            _timer.Start();
        }

        private void timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Switch = false;
        }

        void MonitorSwitch(CancellationToken stoppingToken)
        {
            using (var controller = new GpioController())
            {
                var pin = controller.OpenPin(_switchConfig.GPIO, PinMode.Input);
                while (!stoppingToken.IsCancellationRequested)
                {
                    var value = pin.Read();
                    if (value == PinValue.High)
                    {
                        this.Switch = true;
                    }

                    Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                }
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var switchMonitor = Task.Run(() => MonitorSwitch(stoppingToken));

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
                        if (!Switch)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(100));
                            ssd1306.EnableDisplay(false);
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(_soundConfiguration.ActivationSound) && File.Exists(_soundConfiguration.ActivationSound))
                            await _processRunner.RunAsync(command: "mplayer", workingFolder: _currentFolder, "-volume", volume.ToString(), _soundConfiguration.ActivationSound);

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