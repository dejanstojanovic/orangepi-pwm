using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrangePi.Common.Models;
using OrangePi.Common.Services;
using System.Net;

namespace OrangePi.Common.Extensions
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddBuzzer(this IServiceCollection services, BuzzerConfig config)
        {
            services.TryAddSingleton<IBuzzerService>(new BuzzerService(config));
            return services;
        }

        public static IServiceCollection AddBuzzer(this IServiceCollection services, int pin)
        {
            services.TryAddSingleton<IBuzzerService>(new BuzzerService(pin));
            return services;
        }

        public static IServiceCollection AddProcessRunner(this IServiceCollection services)
        {
            services.TryAddSingleton<IProcessRunner, ProcessRunner>();
            return services;
        }

        public static IServiceCollection AddCpuTemperatureReader(this IServiceCollection services)
        {
            services.AddSingleton<ITemperatureReader, CpuTemperatureReader>();
            return services;
        }

        public static IServiceCollection AddSsdTemperatureReader(this IServiceCollection services,string drive)
        {
            services.AddSingleton<ITemperatureReader>(x=> new SsdTemperatureReader(x.GetRequiredService<IProcessRunner>(), drive));
            return services;
        }

        public static IServiceCollection AddI2CDisplayLock(this IServiceCollection services)
        {
            services.AddTransient<IResourceLock, I2CDisplayLock>();
            return services;
        }
    }
}
