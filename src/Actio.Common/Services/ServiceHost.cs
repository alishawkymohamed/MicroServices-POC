using System;
using Actio.Common.Commands;
using Actio.Common.Events;
using Actio.Common.RabbitMQ;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;

namespace Actio.Common.Services
{
    public class ServiceHost : IServiceHost
    {
        private readonly IWebHost _webHost;

        public ServiceHost(IWebHost webHost)
        {
            _webHost = webHost;
        }
        public void Run() => _webHost.Run();


        public static HostBuilder Create<TStartup>(string[] args) where TStartup : class
        {
            Console.Title = typeof(TStartup).Namespace;
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<TStartup>();

            return new HostBuilder(webHost.Build());
        }

        public abstract class BuilderBase
        {
            public abstract ServiceHost Build();
        }

        public class HostBuilder : BuilderBase
        {
            private readonly IWebHost _webHost;
            private IBusClient _bus;
            public HostBuilder(IWebHost webHost)
            {
                _webHost = webHost;
            }

            public BusBuilder UseRabbitMQ()
            {
                _bus = (IBusClient)_webHost.Services.GetService(typeof(IBusClient));
                return new BusBuilder(_webHost, _bus);
            }
            public override ServiceHost Build()
            {
                return new ServiceHost(_webHost);
            }
        }

        public class BusBuilder : BuilderBase
        {
            private readonly IWebHost _webHost;
            private readonly IBusClient _bus;
            public BusBuilder(IWebHost webHost, IBusClient busClient)
            {
                _webHost = webHost;
                _bus = busClient;
            }

            public BusBuilder SubscribeToCommand<TCommand>() where TCommand : ICommand
            {
                var serviceScopeFactory = _webHost.Services.GetService<IServiceScopeFactory>();
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var handler = (ICommandHandler<TCommand>)scope.ServiceProvider.GetService(typeof(ICommandHandler<TCommand>));
                    _bus.WithCommandHandlerAsync(handler);
                }
                return this;
            }

            public BusBuilder SubscribeToEvent<TEvent>() where TEvent : IEvent
            {
                var serviceScopeFactory = _webHost.Services.GetService<IServiceScopeFactory>();
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var handler = (IEventHandler<TEvent>)scope.ServiceProvider.GetService(typeof(IEventHandler<TEvent>));
                    _bus.WithEventHandlerAsync(handler);
                }

                return this;
            }
            public override ServiceHost Build()
            {
                return new ServiceHost(_webHost);
            }
        }
    }
}