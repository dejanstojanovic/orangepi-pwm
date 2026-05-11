using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OrangePi.Common.Extensions;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models;
using OrangePi.Display.Status.Service.Models.Config;
using OrangePi.Display.Status.Service.Services;
using OrangePi.Display.Status.Service.Services.Info;
using OrangePi.Display.Status.Service.Services.Switch;

namespace OrangePi.Display.Status.Service.Extensions
{
    public static class DependecyInjectionExtensions
    {
        public static IServiceCollection AddScreen(this IServiceCollection services)
        {
            services.AddSingleton<IScreen>(sp =>
            {
                var config = sp.GetRequiredService<IOptions<ScreenConfiguration>>().Value;
                var screen = new Screen(config.BusId, config.DeviceAddress);
                if (config.Rotate)
                    screen.Flip([IScreen.FlipType.Horizontally, IScreen.FlipType.Vertically]);
                return screen;
            });
            return services;
        }

        public static IServiceCollection AddCpuInfo(this IServiceCollection services)
        {
            services.TryAddSingleton<IProcessRunner, ProcessRunner>();
            services.AddCpuTemperatureReader();
            services.AddTransient<IInfoService, CpuInfoService>();
            return services;
        }

        public static IServiceCollection AddRamInfo(this IServiceCollection services)
        {
            services.AddTransient<IInfoService, RamInfoService>();
            return services;
        }

        public static IServiceCollection AddSsdInfo(this IServiceCollection services)
        {
            services.AddSsdTemperatureReader("nvme0");
            services.AddTransient<IInfoService>(x => new SsdInfoService(
               processRunner: x.GetRequiredService<IProcessRunner>(),
               temperatureReaders: x.GetRequiredService<IEnumerable<ITemperatureReader>>(),
               driveMount: "/dev/nvme0n1p2",
               logger: x.GetRequiredService<ILogger<SsdInfoService>>()));

            return services;
        }

        public static IServiceCollection AddInfraredMotionSensorSwitch(this IServiceCollection services, IConfiguration configuration)
        {
            if (services.Any(s => s.ServiceType == typeof(ISwitch)))
                throw new InvalidOperationException($"An {nameof(ISwitch)} is already registered. Only one switch implementation can be registered.");

            services.Configure<InfraredMotionSensorSwitchConfig>(configuration.GetSection($"SwitchConfig:{nameof(InfraredMotionSensorSwitchConfig)}"));
            services.AddSingleton<ISwitch, InfraredMotionSensorSwitch>();
            return services;
        }
        public static IServiceCollection AddProximitySensorSwitch(this IServiceCollection services, IConfiguration configuration)
        {
            if (services.Any(s => s.ServiceType == typeof(ISwitch)))
                throw new InvalidOperationException($"An {nameof(ISwitch)} is already registered. Only one switch implementation can be registered.");

            services.Configure<ProximitySensorSwitchConfig>(configuration.GetSection($"SwitchConfig:{nameof(ProximitySensorSwitchConfig)}"));
            services.AddSingleton<ISwitch, ProximitySensorSwitch>();
            return services;
        }
    }
}
