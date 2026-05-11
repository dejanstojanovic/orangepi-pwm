using Iot.Device.Graphics.SkiaSharpAdapter;
using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using OrangePi.Display.Status.Service.Services;
using OrangePi.Display.Status.Service.Services.Info;
using OrangePi.Display.Status.Service.Services.Switch;
using System.Reflection;

namespace OrangePi.Display.Status.Service
{
    public class Worker : BackgroundService
    {

        readonly string fontName = "DejaVu Sans Bold";
        readonly int fontSize = 12;

        private readonly ILogger<Worker> _logger;
        private readonly ScreenConfiguration _screeConfiguration;
        private readonly SoundConfiguration _soundConfiguration;
        private readonly IProcessRunner _processRunner;
        readonly IEnumerable<IDisplayInfoService> _displayInfoServices;
        readonly string _currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        readonly ISwitch _switch;
        readonly IScreen _screen;
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
            ISwitch @switch,
            IScreen screen
            )
        {
            _logger = logger;
            _displayInfoServices = infoServices.Select(s => s as IDisplayInfoService).ToList();
            _screeConfiguration = serviceConfiguration.Value;
            _soundConfiguration = soundConfig.Value;
            _processRunner = processRunner;
            SkiaSharpAdapter.Register();
            _switch = @switch;
            _screen = screen;
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

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!IsPlaying)
                {
                    _screen.Disable();
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }

                _screen.Enable();

                foreach (var infoService in _displayInfoServices)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    using (var image = await infoService.GetInfoDisplay(_screen.Width, _screen.Height, fontName, fontSize))
                    {
                        _screen.DrawImage(image);
                    }
                    await Task.Delay(pause);
                }
            }

            _screen.Clear();

            await switchMonitor.WaitAsync(stoppingToken);
        }

    }
}