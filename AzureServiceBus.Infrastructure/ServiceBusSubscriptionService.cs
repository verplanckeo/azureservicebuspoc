using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;

namespace AzureServiceBus.Infrastructure
{
    public class ServiceBusSubscriptionService : IServiceBusSubscriptionService, IDisposable
    {
        private Func<MessageReceivedEventArgs, Task> _processMessage;
        private Func<ErrorMessageReceivedEventArgs, Task> _processErrorMessage;

        private readonly IConfiguration _configuration;
        private readonly ServiceBusClient _client;

        private ServiceBusProcessor _messageProcessor;

        private string _queue = "";
        private string _topic = "";
        private string _subscription = "";

        public ServiceBusSubscriptionService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new ServiceBusClient(_configuration["ConnectionStrings:AzureServiceBus"]);
        }

        public event Func<MessageReceivedEventArgs, Task> ProcessMessageAsync
        {
            add
            {
                if (_processMessage != default)
                    throw new NotSupportedException(Constants.ErrorMessages.HandlerIsAlreadyAssigned);

                _processMessage = value;
            }
            remove
            {
                if (_processMessage != value)
                {
                    throw new ArgumentException(Constants.ErrorMessages.HandlerHasNotBeenAssigned);
                }
                _processMessage = default;
            }
        }
        public event Func<ErrorMessageReceivedEventArgs, Task> ProcessErrorMessageAsync
        {
            add
            {
                if (_processErrorMessage != default)
                    throw new NotSupportedException(Constants.ErrorMessages.HandlerIsAlreadyAssigned);

                _processErrorMessage = value;
            }
            remove
            {
                if (_processMessage != value)
                    throw new ArgumentException(Constants.ErrorMessages.HandlerHasNotBeenAssigned);

                _processErrorMessage = default;
            }
        }

        public async Task RegisterHandler(string name, string topic = Constants.ServiceBus.TopicName)
        {
            await RegisterHandler(name, topic, false);
        }

        public async Task RegisterSendOnlyHandler(string topic = Constants.ServiceBus.TopicName)
        {
            await RegisterHandler(null, topic, true);
        }

        private async Task RegisterHandler(string name, string topic, bool isSendOnly)
        {
            var admin = new ServiceBusAdministrationClient(_configuration["ConnectionStrings:AzureServiceBus"]);

            _topic = topic;
            await CreateTopic(_topic, admin);

            if (isSendOnly) return;

            _queue = $"{name}queue";
            _subscription = $"{name}handler";

            await CreateQueue(_queue, admin);
            await CreateSubscription(_topic, _subscription, _queue, admin);
        }

        public async Task StartProcessAsync(CancellationToken cancellationToken)
        {
            await StartMessageProcessorAsync(_queue, cancellationToken);
        }

        private async Task StartMessageProcessorAsync(string queue, CancellationToken cancellationToken)
        {
            _messageProcessor = _client.CreateProcessor(queue);

            _messageProcessor.ProcessMessageAsync += ProcessAsync;
            _messageProcessor.ProcessErrorAsync += ProcessErrorAsync;

            await _messageProcessor.StartProcessingAsync(cancellationToken);
        }

        private async Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            if (_processMessage == null) return;

            await _processErrorMessage.Invoke(new ErrorMessageReceivedEventArgs() { MessageEventArgs = arg });
        }

        private async Task ProcessAsync(ProcessMessageEventArgs arg)
        {
            if (_processMessage == null) return;

            await _processMessage.Invoke(new MessageReceivedEventArgs {MessageEventArgs = arg});

            await arg.CompleteMessageAsync(arg.Message, arg.CancellationToken);
        }

        #region Registration of queues topics subscriptions

        private async Task CreateQueue(string queue, ServiceBusAdministrationClient admin)
        {
            var queueExist = await admin.QueueExistsAsync(queue);
            if (!queueExist)
            {
                var options = new CreateQueueOptions(queue)
                {
                    MaxDeliveryCount = 3
                };
                await admin.CreateQueueAsync(options);
            }
        }

        private async Task CreateTopic(string topic, ServiceBusAdministrationClient admin)
        {
            var topicExist = await admin.TopicExistsAsync(topic);
            if (!topicExist)
            {
                var options = new CreateTopicOptions(topic)
                {
                    MaxSizeInMegabytes = 1024
                };
                await admin.CreateTopicAsync(options);
            }
        }

        private async Task CreateSubscription(string topic, string subscription, string queue, ServiceBusAdministrationClient admin)
        {
            var subscriptionExist = await admin.SubscriptionExistsAsync(topic, subscription);
            if (!subscriptionExist)
            {
                var options = new CreateSubscriptionOptions(topic, subscription)
                {
                    DefaultMessageTimeToLive = new TimeSpan(2, 0, 0, 0),
                    ForwardTo = queue,
                    
                };
                
                await admin.CreateSubscriptionAsync(options);
            }
        }
        
        #endregion

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true).Wait();
            GC.SuppressFinalize(this);
        }

        ~ServiceBusSubscriptionService()
        {
            Dispose(false).Wait();
        }

        protected virtual async Task Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing && _client != null)
            {
                Console.WriteLine($"DISPOSING {nameof(ServiceBusSubscriptionService)}");

                if(_messageProcessor != null)
                    await _messageProcessor.StopProcessingAsync();

                await _client.DisposeAsync();
            }

            _disposed = true;

            Console.WriteLine($"DISPOSED OF {nameof(ServiceBusSubscriptionService)}");
        }

        #endregion
    }
}