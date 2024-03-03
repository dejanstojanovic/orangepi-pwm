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
        public Worker(
            IProcessRunner processRunner,
            ILogger<Worker> logger,
            IOptions<SoundsConfiguration> soundsConfiguration)
        {
            _processRunner = processRunner;
            _logger = logger;
            _soundsConfiguration = soundsConfiguration.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (_soundsConfiguration.Startup.Enabled)
                await _processRunner.RunAsync(command: "mplayer", workingFolder: currentFolder, _soundsConfiguration.Startup.Filename);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            if (_soundsConfiguration.Shutdown.Enabled)
                await _processRunner.RunAsync(command: "mplayer", workingFolder: currentFolder, _soundsConfiguration.Shutdown.Filename);
        }
    }
}
