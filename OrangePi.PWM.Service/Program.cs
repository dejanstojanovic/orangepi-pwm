using Iot.Device.CpuTemperature;
using OrangePi.Common.Services;
using OrangePi.PWM.Service;
using OrangePi.PWM.Service.Models;

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
        services.AddSingleton<IProcessRunner, ProcessRunner>();
        services.AddSingleton<ITemperatureService, TemperatureService>();
        

    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

host.Run();
