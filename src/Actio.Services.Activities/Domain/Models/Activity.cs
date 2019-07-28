using System;
using Actio.Common.Exceptions;

namespace Actio.Services.Activities.Domain.Models
{
    public class Activity
    {
        public Guid Id { get; protected set; }
        public Guid UserId { get; protected set; }
        public string Category { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected Activity()
        {
        }

        public Activity(Guid id, Guid userId, Category category, string name, string description, DateTime createdAt)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ActioException("empty_activity_name", $"Activity name can't be empty");

            Id = id;
            Category = category.Name;
            Name = name;
            Description = description;
            CreatedAt = createdAt;
            UserId = userId;
        }
    }
}