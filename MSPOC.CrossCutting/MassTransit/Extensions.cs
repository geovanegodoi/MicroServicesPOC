using System.Reflection;
using MassTransit;
using MassTransit.Definition;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSPOC.CrossCutting.Settings;
using static MSPOC.CrossCutting.Extensions;

namespace MSPOC.CrossCutting.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services, Assembly assembly)
        {
            services.AddMassTransit(config => 
            {
                config.AddConsumers(assembly);
                
                config.UsingRabbitMq((context, cfg) =>
                {
                    var configuration = context.GetConfiguration();
                    cfg.ConfigureEndpoints(context, configuration);
                    cfg.ConfigureHost(configuration);
                });
            });

            services.AddMassTransitHostedService();

            return services;
        }

        private static void ConfigureEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context, IConfiguration configuration)
        {
            var serviceSettings = configuration.GetSection<ServiceSettings>();
            GuardIsNotNull(serviceSettings);
            cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
        }

        private static void ConfigureHost(this IRabbitMqBusFactoryConfigurator cfg, IConfiguration configuration)
        {
            var rabbitMQSettings = configuration.GetSection<RabbitMQSettings>();
            GuardIsNotNull(rabbitMQSettings);
            cfg.Host(rabbitMQSettings.Host);
        }
    }
}