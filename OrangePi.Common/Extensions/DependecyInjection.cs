using Microsoft.Extensions.DependencyInjection;
using OrangePi.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Common.Extensions
{
    public static class DependecyInjection
    {
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
            services.AddHttpClient("glances", c =>
            {
                c.BaseAddress = apiUrl;
            });
            services.AddSingleton<IGlancesService, GlancesService>();
            return services;
        }

        public static IServiceCollection AddGlancesService(this IServiceCollection services, string apiUrl)
        {
            return services.AddGlancesService(new Uri(apiUrl));
        }
    }
}
