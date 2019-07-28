using System;
using System.Threading.Tasks;

namespace Actio.Services.Activities.Services
{
    public interface IActivityService
    {
        Task AddAsync(Guid id, Guid UserId, string category, string name, string description, DateTime createdAt);
    }
}