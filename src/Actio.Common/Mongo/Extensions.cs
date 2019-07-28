using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Actio.Common.Mongo
{
    public static class Extensions
    {
        public static void AddMongoDB(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoOptions>(configuration.GetSection("mongo"));

            services.AddSingleton<MongoClient>(cconfig =>
            {
                var options = cconfig.GetService<IOptions<MongoOptions>>();
                return new MongoClient(options.Value.ConnectionString);
            });

            services.AddScoped<IMongoDatabase>(config =>
            {
                var options = config.GetService<IOptions<MongoOptions>>();
                var client = config.GetService<MongoClient>();

                return client.GetDatabase(options.Value.DatabaseName);
            });

            services.AddScoped<IDatabaseIntializer, MongoIntializer>();
            services.AddScoped<IDatabaseSeeder, MongoSeeder>();
        }
    }
}