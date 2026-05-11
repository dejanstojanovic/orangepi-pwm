using OrangePi.Common.Extensions;
using OrangePi.Common.Models;
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

        var buzzerConfig = new BuzzerConfig();
        hostContext.Configuration.Bind(nameof(buzzerConfig), buzzerConfig);

        services.AddProcessRunner()
                .AddCpuTemperatureReader()
                .AddSsdTemperatureReader("nvme0")
                .AddBuzzer(buzzerConfig);
        
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

host.Run();
