using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MSPOC.CrossCutting.Settings;
using static MSPOC.CrossCutting.Extensions;

namespace MSPOC.CrossCutting.MongoDb
{
    public static class Extensions
    {
        public static IServiceCollection AddMongoDatabase(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddSingleton(serviceProvider =>
            {
                var configuration   = serviceProvider.GetConfiguration();
                
                var mongoDbSettings = configuration.GetSection<MongoDbSettings>();
                GuardIsNotNull(mongoDbSettings);
                
                var serviceSettings = configuration.GetSection<ServiceSettings>();
                GuardIsNotNull(serviceSettings);
                
                var mongoClient     = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
            where T : Entity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });
            return services;
        }
    }
}