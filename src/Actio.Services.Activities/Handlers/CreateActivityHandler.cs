using System;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Actio.Common.Events;
using Actio.Common.Exceptions;
using Actio.Services.Activities.Services;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace Actio.Services.Activities.Handlers
{
    public class CreateActivityHandler : ICommandHandler<CreateActivity>
    {
        private readonly IBusClient _busClient;
        private readonly IActivityService _activityService;
        private readonly ILogger<CreateActivityHandler> _logger;

        public CreateActivityHandler(IBusClient busClient, IActivityService activityService, ILogger<CreateActivityHandler> logger)
        {
            _busClient = busClient;
            _activityService = activityService;
            _logger = logger;
        }
        public async Task HandleAsync(CreateActivity Command)
        {
            _logger.LogInformation($"Creating Activity: {Command.Name} !");
            try
            {
                await _activityService.AddAsync(Command.Id, Command.UserId, Command.Category, Command.Name, Command.Description, Command.CreatedAt);
                await _busClient.PublishAsync(new ActivityCreated(Command.Id, Command.UserId, Command.Category, Command.Name, Command.Description, Command.CreatedAt));
            }
            catch (ActioException ex)
            {
                await _busClient.PublishAsync(new CreateActivityRejected(Command.Id, ex.Message, ex.Code));
                _logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                await _busClient.PublishAsync(new CreateActivityRejected(Command.Id, ex.Message, "error"));
                _logger.LogError(ex.Message);
            }
        }
    }
}