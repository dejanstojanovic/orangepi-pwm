using Microsoft.Extensions.Options;
using OrangePi.Common.Services;
using OrangePi.Startup.Sound.Service.Models;
using System.Reflection;

namespace OrangePi.Startup.Sound.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IProcessRunner _processRunner;
        private readonly SoundsConfiguration _soundsConfiguration;
        private readonly string _currentFolder;
        private readonly int _volume = 80;
        public Worker(
            IProcessRunner processRunner,
            ILogger<Worker> logger,
            IOptions<SoundsConfiguration> soundsConfiguration)
        {
            _processRunner = processRunner;
            _logger = logger;
            _soundsConfiguration = soundsConfiguration.Value;
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_soundsConfiguration.Startup.Enabled)
                await _processRunner.RunAsync(command: "mplayer", workingFolder: _currentFolder, "-volume", _volume.ToString(), _soundsConfiguration.Startup.Filename);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            if (_soundsConfiguration.Shutdown.Enabled)
                await _processRunner.RunAsync(command: "mplayer", workingFolder: _currentFolder, "-volume", _volume.ToString(), _soundsConfiguration.Shutdown.Filename);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }
    }
}
