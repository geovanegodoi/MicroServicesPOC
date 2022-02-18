using System.Linq;
using System.Net;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSPOC.CrossCutting.Settings;
using static MSPOC.CrossCutting.Extensions;

namespace MSPOC.CrossCutting.HealthCheck
{
    public static class Extensions
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration   = serviceProvider.GetConfiguration();
            
            services.AddCheckForMongoDb(configuration)
                    .AddCheckForRabbitMQ(configuration)
                    .AddCustomHealthChecksUI(configuration);

            return services;
        }

        private static IServiceCollection AddCheckForMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings  = configuration.GetSection<MongoDbSettings>();
            GuardIsNotNull(mongoDbSettings);
            
            services.AddHealthChecks()
                    .AddMongoDb(
                        mongodbConnectionString: mongoDbSettings.ConnectionString, 
                        name: "mongodb");

            return services;
        }

        private static IServiceCollection AddCheckForRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqSettings = configuration.GetSection<RabbitMQSettings>();
            GuardIsNotNull(rabbitMqSettings);
            
            services.AddHealthChecks()
                    .AddRabbitMQ(
                        rabbitConnectionString: $"amqp://guest:guest@{rabbitMqSettings.Host}:5672", 
                        name: "rabbitmq");

            return services;
        }

        private static IServiceCollection AddCustomHealthChecksUI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecksUI(setup =>
            {
                var endpointName = Assembly.GetEntryAssembly().GetName().Name;
                var hostName     = Dns.GetHostName();
                var portNumber   = configuration["urls"].Split(":").LastOrDefault();
                setup.AddHealthCheckEndpoint(endpointName, $"http://{hostName}:{portNumber}/healthchecks-data-ui");
            })
            .AddInMemoryStorage();

            return services;
        } 

        public static IApplicationBuilder UseCustomHealthCheckUI(this IApplicationBuilder app, string css = "")
        {
            // Gera o endpoint que retornará os dados utilizados no dashboard
            app.UseHealthChecks("/healthchecks-data-ui", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // Ativa o dashboard para a visualização da situação de cada Health Check
            app.UseHealthChecksUI(setup =>
            {
                setup.UIPath  = "/healthchecks-ui";
                setup.ApiPath = "/healthchecks-ui-api";
                setup.AddCustomStylesheet(string.IsNullOrWhiteSpace(css) ? "dotnet.css" : css);
            });

            return app;
        }
    }
}