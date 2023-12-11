using Microsoft.Extensions.DependencyInjection;
using OrangePi.Common.Models;
using OrangePi.Common.Services;

namespace OrangePi.Common.Extensions
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddBuzzer(this IServiceCollection services, BuzzerConfig config)
        {
            services.AddSingleton<IBuzzerService>(new BuzzerService(config));
            return services;
        }

        public static IServiceCollection AddBuzzer(this IServiceCollection services, int gpio, int pwmFrequency, double pwmDuty)
        {
            services.AddSingleton<IBuzzerService>(new BuzzerService(gpio, pwmFrequency, pwmDuty));
            return services;
        }

        public static IServiceCollection AddProcessRunner(this IServiceCollection services)
        {
            services.AddSingleton<IProcessRunner, ProcessRunner>();
            services.AddSingleton<ITemperatureService, TemperatureService>();
            return services;
        }

        public static IServiceCollection AddTemperatureCheck(this IServiceCollection services)
        {
            services.AddSingleton<ITemperatureService, TemperatureService>();
            return services;
        }

        public static IServiceCollection AddGlancesService(this IServiceCollection services, Uri apiUrl)
        {
            services.AddSingleton<IGlancesService, GlancesService>();
            services.AddHttpClient<IGlancesService, GlancesService>(c =>
            {
                c.BaseAddress = apiUrl;
            });
            return services;
        }

        public static IServiceCollection AddGlancesService(this IServiceCollection services, string apiUrl)
        {
            return services.AddGlancesService(new Uri(apiUrl));
        }
    }
}
