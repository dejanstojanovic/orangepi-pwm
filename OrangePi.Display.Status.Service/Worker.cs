using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Extensions;
using OrangePi.Display.Status.Service.InfoServices;
using OrangePi.Display.Status.Service.Models;
using SkiaSharp;
using System.Device.Gpio;
using System.Device.I2c;
using System.Drawing;

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

        int screenWidth = 128;
        int screenHeight = 64;
        string fontName = "DejaVu Sans Bold";
        int fontSize = 12;

        private readonly ILogger<Worker> _logger;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly SwitchConfig _switchConfig;
        readonly System.Timers.Timer _timer;
        readonly IEnumerable<IDisplayInfoService> _infoServices;
        public Worker(
            ILogger<Worker> logger,
            IOptions<ServiceConfiguration> serviceConfiguration,
            IOptions<SwitchConfig> switchConfig,
            IEnumerable<IDisplayInfoService> infoServices
            )
        {
            _logger = logger;
            _infoServices = infoServices;
            _serviceConfiguration = serviceConfiguration.Value;
            _switchConfig = switchConfig.Value;

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

                        ssd1306.EnableDisplay(true);

                        foreach (var infoService in _infoServices)
                        {
                            if (stoppingToken.IsCancellationRequested)
                                break;

                            await Task.Delay(pause);

                            using (var image = await infoService.GetDisplay(screenWidth, screenHeight, fontName, fontSize))
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