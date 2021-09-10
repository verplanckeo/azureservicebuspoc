using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.Infrastructure;
using AzureServiceBus.Messaging.Seedwork;
using AzureServiceBus.Messaging.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AzureServiceBusPoc.Publisher
{
    public class ProgramRunner : IProgramRunner, IHostedService
    {
        private readonly IServiceBusSubscriptionService _subscription;
        private readonly IServiceBusPublisher _publisher;

        public ProgramRunner(IServiceBusSubscriptionService subscription, IServiceBusPublisher publisher)
        {
            _subscription = subscription;
            _publisher = publisher;
        }
        public async Task RunAsync()
        {
            Console.WriteLine("Press <Enter> to exit ...");

            var stop = false;
            do
            {
                Console.WriteLine("Press <1> to send UserRegisteredCommand ...");
                Console.WriteLine("Press <2> to publish UserRegisteredEvent ...");
                Console.WriteLine("Give input: ");
                var key = Console.ReadKey().Key;
                Console.WriteLine();

                switch (key)
                {
                    case ConsoleKey.D1:
                        await _publisher.SendAsync(UserRegisteredCommand.Create("olivier", new List<SocialMediaPlatform> { SocialMediaPlatform.Facebook }, "facebookqueue"), default);
                        break;
                    case ConsoleKey.D2:
                        await _publisher.PublishAsync(UserRegisteredEvent.Create("olivier"), default);
                        break;
                    case ConsoleKey.Enter:
                        stop = true;
                        break;
                }
            } while (!stop);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _subscription.RegisterSendOnlyHandler();
            await RunAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}