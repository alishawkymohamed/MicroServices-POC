using System;
using System.Threading.Tasks;
using Actio.Api.DTOs;
using Actio.Api.Repositories;
using Actio.Common.Events;

namespace Actio.Api.Handlers
{
    public class ActivityCreatedHandler : IEventHandler<ActivityCreated>
    {
        private readonly IActivityRepository _repository;

        public ActivityCreatedHandler(IActivityRepository repository)
        {
            _repository = repository;
        }
        public async Task HandleAsync(ActivityCreated Event)
        {
            await _repository.AddAsync(new Activity
            {
                Id = Event.Id,
                Category = Event.Category,
                CreatedAt = Event.CreatedAt,
                Description = Event.Description,
                Name = Event.Name,
                UserId = Event.UserId
            });
        }
    }
}