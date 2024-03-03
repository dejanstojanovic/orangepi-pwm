using OrangePi.Startup.Sound.Service;
using OrangePi.Common.Extensions;
using OrangePi.Common.Models;
using OrangePi.Startup.Sound.Service.Models;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddProcessRunner();
builder.Services.Configure<SoundsConfiguration>(builder.Configuration.GetSection(nameof(SoundsConfiguration)));
var host = builder.Build();
host.Run();
