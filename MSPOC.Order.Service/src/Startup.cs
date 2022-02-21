using System.Reflection;
using AutoMapper;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MSPOC.CrossCutting;
using MSPOC.CrossCutting.HealthCheck;
using MSPOC.CrossCutting.MassTransit;
using MSPOC.CrossCutting.MongoDb;
using MSPOC.CrossCutting.Settings;
using MSPOC.Order.Service.Consumers;
using MSPOC.Order.Service.Entities;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDatabase()
                    .AddMongoRepository<Entity.Order>("orders")
                    .AddMongoRepository<Entity.CatalogItem>("catalogitems")
                    .AddMongoRepository<Entity.Customer>("customers");
            
            services.AddAutoMapper(typeof(Startup));

            services.AddMassTransitWithRabbitMQ(Assembly.GetEntryAssembly());

            services.AddCustomHealthCheck();

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MSPOC.Order.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MSPOC.Order.Service v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCustomHealthCheckUI();
        }
    }
}
