using System;
using System.Linq;
using System.Threading.Tasks;
using Actio.Api.Repositories;
using Actio.Common.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RawRabbit;

namespace Actio.Api.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ActivitiesController : Controller
    {
        private readonly IBusClient _busClient;
        private readonly IActivityRepository _repository;

        public ActivitiesController(IBusClient busClient,
        IActivityRepository repository)
        {
            _busClient = busClient;
            _repository = repository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var activities = await _repository.BrowseAsync(Guid.Parse(User.Identity.Name));
            return Json(activities.Select(x => new { x.Id, x.Name, x.Category }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var activity = await _repository.GetAsync(id);

            if (activity == null)
                return NotFound();

            if (activity.UserId != Guid.Parse(User.Identity.Name))
                return Unauthorized();

            return Json(new { activity.Id, activity.Name, activity.Category });
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]CreateActivity command)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                return Unauthorized();

            command.Id = Guid.NewGuid();
            command.CreatedAt = DateTime.UtcNow;
            command.UserId = Guid.Parse(User.Identity.Name);
            await _busClient.PublishAsync<CreateActivity>(command);
            return Accepted($"Activities/{command.Id}");
        }
    }
}