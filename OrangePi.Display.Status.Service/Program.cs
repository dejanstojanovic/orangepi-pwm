using Iot.Device.Graphics.SkiaSharpAdapter;
using OrangePi.Common.Extensions;
using OrangePi.Display.Status.Service;
using OrangePi.Display.Status.Service.Extensions;
using OrangePi.Display.Status.Service.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        SkiaSharpAdapter.Register();

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
                services.AddLogging();
                services.Configure<ScreenConfiguration>(hostContext.Configuration.GetSection(nameof(ScreenConfiguration)));
                services.Configure<SoundConfiguration>(hostContext.Configuration.GetSection(nameof(SoundConfiguration)));
                services.AddScreen();
                services.AddHostedService<Worker>();

                services.AddProcessRunner();

                services.AddCpuInfo();
                services.AddRamInfo();
                services.AddSsdInfo();

                services.AddInfraredMotionSensorSwitch(hostContext.Configuration);

            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();

        host.Run();
    }
}