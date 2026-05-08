using Microsoft.Extensions.DependencyInjection.Extensions;
using OrangePi.Common.Extensions;
using OrangePi.Common.Services;
using OrangePi.Display.Status.Service.Models.Config;
using OrangePi.Display.Status.Service.Services.Info;
using OrangePi.Display.Status.Service.Services.Switch;

namespace OrangePi.Display.Status.Service.Extensions
{
    public static class DependecyInjectionExtensions
    {
        public static IServiceCollection AddCpuInfo(this IServiceCollection services)
        {
            services.TryAddSingleton<IProcessRunner, ProcessRunner>();
            services.AddCpuTemperatureReader();
            services.TryAddTransient<IInfoService, CpuInfoService>();
            return services;
        }

        public static IServiceCollection AddRamInfo(this IServiceCollection services)
        {
            services.TryAddTransient<IInfoService, RamInfoService>();
            return services;
        }

        public static IServiceCollection AddSsdInfo(this IServiceCollection services)
        {
            services.AddSsdTemperatureReader("nvme0");
            services.TryAddTransient<IInfoService>(x => new SsdInfoService(
               processRunner: x.GetRequiredService<IProcessRunner>(),
               temperatureReaders: x.GetRequiredService<IEnumerable<ITemperatureReader>>(),
               driveMount: "/dev/nvme0n1p2",
               logger: x.GetRequiredService<ILogger<SsdInfoService>>()));

            return services;
        }

        public static IServiceCollection AddInfraredMotionSensorSwitch(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<InfraredMotionSensorSwitchConfig>(configuration.GetSection($"SwitchConfig:{nameof(InfraredMotionSensorSwitchConfig)}"));
            services.TryAddSingleton<ISwitch, InfraredMotionSensorSwitch>();
            return services;
        }
        public static IServiceCollection AddProximitySensorSwitch(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ProximitySensorSwitchConfig>(configuration.GetSection($"SwitchConfig:{nameof(ProximitySensorSwitchConfig)}"));
            services.TryAddSingleton<ISwitch, ProximitySensorSwitch>();
            return services;
        }
    }
}
