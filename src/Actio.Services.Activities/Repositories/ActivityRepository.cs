using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Actio.Services.Activities.Domain.Models;
using Actio.Services.Activities.Domain.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Actio.Services.Activities.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public ActivityRepository(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }
        public async Task<Activity> GetAsync(Guid id)
            => await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
        public async Task AddAsync(Activity activity)
           => await Collection.InsertOneAsync(activity);

        public async Task<IEnumerable<Activity>> BrowseAsync()
            => await Collection.AsQueryable().ToListAsync();

        private IMongoCollection<Activity> Collection => _mongoDatabase.GetCollection<Activity>("Activities");
    }
}