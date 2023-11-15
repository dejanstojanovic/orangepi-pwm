using OrangePi.Display.Status.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<App>();
    })
    .Build();

host.Run();
