// https://learn.microsoft.com/en-us/dotnet/iot/tutorials/lcd-display

using Microsoft.Extensions.DependencyInjection;
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

        services.AddCpuTemperatureReader();
        services.AddSsdTemperatureReader("nvme0");

        services.AddGlancesService(hostContext.Configuration.GetValue<string>("Glances:Url"));
        services.AddPiHole(new OrangePi.Common.Models.PiHoleConfig
        {
            Url = new Uri(hostContext.Configuration.GetValue<string>("PiHole:Url")),
            Key = hostContext.Configuration.GetValue<string>("PiHole:Key")
        });

        services.AddSingleton<IInfoService, CpuInfoService>();
        services.AddSingleton<IInfoService, RamInfoService>();
        services.AddSingleton<IInfoService>(x => new SsdInfoService(
            x.GetRequiredService<IProcessRunner>(), 
            x.GetRequiredService<IEnumerable<ITemperatureReader>>(), 
            "/dev/nvme0n1p2"));
        

        services.AddSingleton<IHostInfoService>(x => new HostInfoService(x.GetRequiredService<IProcessRunner>(), "end1"));
        services.AddSingleton<IDateTimeInfoService, DateTimeInfoService>();

    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

host.Run();
