// https://learn.microsoft.com/en-us/dotnet/iot/tutorials/lcd-display

using OrangePi.Common.Extensions;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service;
using OrangePi.Display.Status.Service.Models;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions();
        services.Configure<ServiceConfiguration>(hostContext.Configuration.GetSection(nameof(ServiceConfiguration)));
        services.Configure<SwitchConfig>(hostContext.Configuration.GetSection(nameof(SwitchConfig)));
        services.AddHostedService<Worker>();
        services.AddSingleton<IProcessRunner, ProcessRunner>();
        services.AddSingleton<ITemperatureService, TemperatureService>();
        services.AddGlancesService("http://192.168.1.25:61208/");
        services.AddPiHole(new OrangePi.Common.Models.PiHoleConfig
        {
            Url = new Uri("https://pihole.myhomeserver.local/"),
            Key = "39a2885f0dea20dc57854225bce0a571bd2a7cd6c6d2f97873be6fb1261fd914"
        });
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

host.Run();
