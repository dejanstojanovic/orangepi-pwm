using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrangePi.Common.Models;
using OrangePi.Common.Services;
using System.Net;

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

        public static IServiceCollection AddGlancesService(this IServiceCollection services, Uri apiUrl)
        {
            services.AddSingleton<IGlancesClient, GlancesClient>();
            services.AddHttpClient<IGlancesClient, GlancesClient>(c =>
            {
                c.BaseAddress = apiUrl;
            });
            return services;
        }

        public static IServiceCollection AddGlancesService(this IServiceCollection services, string apiUrl)
        {
            return services.AddGlancesService(new Uri(apiUrl));
        }

        public static IServiceCollection AddPiHole(this IServiceCollection services, PiHoleConfig piHoleConfig)
        {
            services.AddSingleton<IPiHoleService, PiHoleService>();
            services.Configure<PiHoleConfig>(o =>
            {
                o.Url = piHoleConfig.Url;
                o.Key = piHoleConfig.Key;
            });

            services.AddHttpClient<IPiHoleService, PiHoleService>(c =>
            {
                c.BaseAddress = piHoleConfig.Url;
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) =>
                    {
                        return true;
                    }
                };
                return handler;
            });
            return services;
        }
        public static IServiceCollection AddPiHole(this IServiceCollection services, ConfigurationSection configSection)
        {
            var config = new PiHoleConfig();
            configSection.Bind(config);
            services.AddPiHole(config);

            return services;
        }
    }
}
