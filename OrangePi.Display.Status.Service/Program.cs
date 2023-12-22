// https://learn.microsoft.com/en-us/dotnet/iot/tutorials/lcd-display

using OrangePi.Common.Extensions;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service;
using OrangePi.Display.Status.Service.InfoServices;
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
        services.AddGlancesService(hostContext.Configuration.GetValue<string>("Glances:Url"));
        services.AddPiHole(new OrangePi.Common.Models.PiHoleConfig
        {
            Url = new Uri(hostContext.Configuration.GetValue<string>("PiHole:Url")),
            Key = hostContext.Configuration.GetValue<string>("PiHole:Key")
        });

        services.Scan(selector => selector
            .FromCallingAssembly()
            .AddClasses(
                classSelector =>
                    classSelector.InNamespaces(typeof(IInfoService).Namespace)
            ).AsImplementedInterfaces()
); ;
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

host.Run();
