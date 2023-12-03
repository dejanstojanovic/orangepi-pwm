using Iot.Device.CpuTemperature;
using OrangePi.Common.Extensions;
using OrangePi.Common.Services;
using OrangePi.Fan.Service;
using OrangePi.Fan.Service.Models;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions();
        services.Configure<ServiceConfiguration>(hostContext.Configuration.GetSection(nameof(ServiceConfiguration)));
        services.AddHostedService<Worker>();

        services.AddProcessRunner()
                .AddTemperatureCheck()
                .AddBuzzer(138, 1000400, 0.9);
        
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

host.Run();
