using Actio.Common.Commands;
using Actio.Common.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Instantiation;
using System.Reflection;
using System.Threading.Tasks;

namespace Actio.Common.RabbitMQ
{
    public static class Extenstions
    {
        public static Task WithCommandHandlerAsync<TCommand>(this IBusClient busClient, ICommandHandler<TCommand> handler) where TCommand : ICommand
        {
            return busClient.SubscribeAsync<TCommand>(
                msg => handler.HandleAsync(msg),
                context => context.UseSubscribeConfiguration(
                cfg => cfg.FromDeclaredQueue(
                queue => queue.WithName(GetQueueName<TCommand>()))));
        }

        public static Task WithEventHandlerAsync<TEvent>(this IBusClient busClient, IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            return busClient.SubscribeAsync<TEvent>(msg =>
                    handler.HandleAsync(msg),
                    context => context.UseSubscribeConfiguration(cfg =>
                    cfg.FromDeclaredQueue(queue => queue.WithName(GetQueueName<TEvent>()))));
        }

        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new RabbitMqOptions();
            configuration.GetSection("rabbitmq").Bind(options);

            var client = RawRabbitFactory.CreateSingleton(new RawRabbitOptions
            {
                ClientConfiguration = options
            });
            services.AddSingleton<IBusClient>(_ => client);
            // services.AddRawRabbit(new RawRabbitOptions
            // {
            //     ClientConfiguration = options
            // });
        }

        private static string GetQueueName<T>()
            => $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}";
    }
}