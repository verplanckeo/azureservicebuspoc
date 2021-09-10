using System;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureServiceBus.Messaging.Seedwork;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AzureServiceBus.Infrastructure
{
    public class ServiceBusPublisher : IServiceBusPublisher, IDisposable
    {
        private readonly ServiceBusClient _client;

        public ServiceBusPublisher(IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:AzureServiceBus"];
            _client = new ServiceBusClient(connectionString);
        }

        public async Task SendAsync(ICommand command, CancellationToken cancellationToken)
        {
            var sender = _client.CreateSender(command.Queue);
            var message = CreateServiceBusMessage(command);

            await sender.SendMessageAsync(message, cancellationToken);

            Console.WriteLine($"Sent message {message.Body} to the queue '{command.Queue}'");
        }

        public async Task PublishAsync(IEvent evt, CancellationToken cancellationToken)
        {
            var sender = _client.CreateSender(evt.Topic);
            var message = CreateServiceBusMessage(evt);

            await sender.SendMessageAsync(message, cancellationToken);

            Console.WriteLine($"Sent event {message.Body} to the queue '{evt.Topic}'");
        }

        private ServiceBusMessage CreateServiceBusMessage(IMessage msg)
        {
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(msg));
            message.ApplicationProperties.Add(Constants.MessageHeaders.MessageTypeKey, message.GetType().FullName);
            
            return message;
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true).Wait();
            GC.SuppressFinalize(this);
        }

        ~ServiceBusPublisher()
        {
            Dispose(false).Wait();
        }

        protected virtual async Task Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing && _client != null)
            {
                Console.WriteLine($"DISPOSING {nameof(ServiceBusPublisher)}");

                await _client.DisposeAsync();
            }

            _disposed = true;
            Console.WriteLine($"DISPOSED OF {nameof(ServiceBusPublisher)}");
        }

        #endregion
    }
}
