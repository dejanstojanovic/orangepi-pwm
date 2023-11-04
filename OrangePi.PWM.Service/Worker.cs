using Microsoft.Extensions.Options;
using OrangePi.PWM.Service.Models;

namespace OrangePi.PWM.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceConfiguration _serviceConfiguration;
        public Worker(ILogger<Worker> logger, IOptions<ServiceConfiguration> serviceConfiguration)
        {
            _logger = logger;
            _serviceConfiguration = serviceConfiguration.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                Console.WriteLine(_serviceConfiguration.wPi);
                Task.Delay(1000).Wait();
            }
        }
    }
}