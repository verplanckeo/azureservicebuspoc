using System;
using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace AzureServiceBus.GoogleHandler
{
    public class MessageHandler : IHostedService
    {
        private readonly IServiceBusSubscriptionService _subscriptionService;

        public MessageHandler(IServiceBusSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _subscriptionService.RegisterHandler("google");

            _subscriptionService.ProcessMessageAsync += SubscriptionService_ProcessMessageAsync;
            _subscriptionService.ProcessErrorMessageAsync += SubscriptionService_ProcessErrorMessageAsync;

            await _subscriptionService.StartProcessAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private Task SubscriptionService_ProcessMessageAsync(MessageReceivedEventArgs arg)
        {
            Console.WriteLine($"Type of event: {arg.MessageEventArgs.Message.ApplicationProperties[Constants.MessageHeaders.MessageTypeKey]}");
            Console.WriteLine($"Processing event: {arg.MessageEventArgs.Message.Body.ToString()}");

            return Task.CompletedTask;
        }

        private Task SubscriptionService_ProcessErrorMessageAsync(ErrorMessageReceivedEventArgs arg)
        {
            Console.WriteLine(arg.MessageEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
}