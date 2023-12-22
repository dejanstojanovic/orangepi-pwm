using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
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
        readonly IEnumerable<IInfoService> _infoServices;
        public Worker(
            ILogger<Worker> logger,
            IOptions<ServiceConfiguration> serviceConfiguration,
            IOptions<SwitchConfig> switchConfig,
            IEnumerable<IInfoService> infoServices
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
                            var value = await infoService.GetValue();

                            using (var image = BitmapImage.CreateBitmap(screenWidth, screenHeight, PixelFormat.Format32bppArgb))
                            {
                                image.Clear(Color.Black);
                                var graphic = image.GetDrawingApi();
                                var canvas = graphic.GetCanvas();

                                //Draw border
                                canvas.DrawArc(new SKRect(0, 0, screenHeight, screenHeight / 2), 0, 360, true, new SKPaint() { Color = SKColor.Parse("FFFFFF") });
                                canvas.DrawArc(new SKRect(1, 1, screenHeight - 1, screenHeight / 2 - 1), 0, 360, true, new SKPaint() { Color = SKColor.Parse("000000") });

                                //Draw value
                                var angle = (int)Math.Round(((value.Value < 100 ? value.Value : 100) / 100) * 360);
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
                                    labelPaint.MeasureText(infoService.Label, ref sizeRect);
                                    graphic.DrawText(text: infoService.Label,
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
                                    SKRect valueSizeRect = new();
                                    valuePaint.MeasureText(value.ValueText, ref valueSizeRect);
                                    graphic.DrawText(text: value.ValueText,
                                        fontFamilyName: fontName,
                                        size: (int)valuePaint.TextSize,
                                        color: Color.White,
                                        position: new Point(
                                            x: (screenHeight + (int)(screenHeight - valueSizeRect.Width)) - 3,
                                            y: (screenHeight / 4) - ((fontSize + 5) - 2) + 3)
                                        );
                                    //Draw note
                                    if (!string.IsNullOrWhiteSpace(value.Note))
                                    {
                                        using (var notePaint = new SKPaint
                                        {
                                            TextSize = 10,
                                        })
                                        {
                                            SKRect noteSizeRect = new();
                                            notePaint.MeasureText(value.Note, ref noteSizeRect);
                                            graphic.DrawText(text: value.Note,
                                                fontFamilyName: fontName,
                                                size: 10,
                                                color: Color.White,
                                                position: new Point(
                                                    x: (screenHeight + (int)(screenHeight - noteSizeRect.Width)),
                                                    y: (screenHeight / 2) - fontSize)
                                        );
                                        }
                                    }
                                }
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