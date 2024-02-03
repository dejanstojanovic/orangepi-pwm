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

        services.AddTransient<IInfoService, CpuInfoService>();
        services.AddTransient<IInfoService, RamInfoService>();
        services.AddTransient<IInfoService>(x => new SsdInfoService(
           processRunner: x.GetRequiredService<IProcessRunner>(),
           temperatureReaders: x.GetRequiredService<IEnumerable<ITemperatureReader>>(),
           driveMount: "/dev/nvme0n1p2"));

        services.AddTransient<IHostInfoService>(x => new HostInfoService(
           processRunner: x.GetRequiredService<IProcessRunner>(),
           networkAdapter: "end1"));
        services.AddTransient<IDateTimeInfoService, DateTimeInfoService>();

    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

host.Run();
