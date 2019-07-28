using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Actio.Api.DTOs;

namespace Actio.Api.Repositories
{
    public interface IActivityRepository
    {
        Task AddAsync(Activity model);
        Task<Activity> GetAsync(Guid id);
        Task<IEnumerable<Activity>> BrowseAsync(Guid? userId);
    }
}