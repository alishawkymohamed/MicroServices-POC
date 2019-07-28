using System;
using System.Threading.Tasks;
using Actio.Common.Exceptions;
using Actio.Services.Activities.Domain.Models;
using Actio.Services.Activities.Domain.Repositories;

namespace Actio.Services.Activities.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ActivityService(IActivityRepository activityRepository, ICategoryRepository categoryRepository)
        {
            _activityRepository = activityRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task AddAsync(Guid id, Guid userId, string category, string name, string description, DateTime createdAt)
        {
            var activityCategry = await _categoryRepository.GetAsync(category);
            if (activityCategry == null)
            {
                await _categoryRepository.AddAsync(new Category(category));
                // throw new ActioException("category_not_found",$"Category {category} not found");
            }

            await _activityRepository.AddAsync(new Activity(id, userId, activityCategry, name, description, createdAt));
        }
    }
}