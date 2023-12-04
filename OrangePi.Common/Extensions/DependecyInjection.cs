using Microsoft.Extensions.DependencyInjection;
using OrangePi.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
