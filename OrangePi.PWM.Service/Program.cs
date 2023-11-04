using OrangePi.PWM.Service;
using OrangePi.PWM.Service.Models;
using System.Runtime;
using Microsoft.Extensions.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);

        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions();
        services.Configure<ServiceConfiguration>(hostContext.Configuration.GetSection(nameof(ServiceConfiguration)));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
