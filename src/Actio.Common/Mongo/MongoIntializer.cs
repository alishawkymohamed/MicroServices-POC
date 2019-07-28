using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Actio.Common.Mongo
{
    public class MongoIntializer : IDatabaseIntializer
    {
        private bool _intialized;
        private readonly bool _seed;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IDatabaseSeeder _seeder;

        public MongoIntializer(IMongoDatabase mongoDatabase, IOptions<MongoOptions> options, IDatabaseSeeder seeder)
        {
            _seeder = seeder;
            _mongoDatabase = mongoDatabase;
            _seed = options.Value.Seed;
        }
        public async Task IntializeAsync()
        {
            if (_intialized)
                return;

            await RegisterConventions();
            _intialized = true;

            if (!_seed)
                return;

            await _seeder.SeedAsync();
        }

        private async Task RegisterConventions()
        {
            await Task.Run(() => ConventionRegistry.Register("ActioConvention", new MongoConventions(), x => true));
        }

        private class MongoConventions : IConventionPack
        {
            public IEnumerable<IConvention> Conventions => new List<IConvention>
            {
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new CamelCaseElementNameConvention()
            };
        }
    }
}