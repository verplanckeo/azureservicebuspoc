using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureServiceBus.Infrastructure
{
    public interface IServiceBusSubscriptionService
    {
        /// <summary>
        /// Event published when a message is received
        /// </summary>
        event Func<MessageReceivedEventArgs, Task> ProcessMessageAsync;

        /// <summary>
        /// Event published when an error message is received
        /// </summary>
        event Func<ErrorMessageReceivedEventArgs, Task> ProcessErrorMessageAsync;

        /// <summary>
        /// Register a handler which processes messages.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task RegisterHandler(string name, string topic = Constants.ServiceBus.TopicName);

        /// <summary>
        /// Register handler for processes which do not process any messages.
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task RegisterSendOnlyHandler(string topic = Constants.ServiceBus.TopicName);


        /// <summary>
        /// Start processing messages applicable for the handler.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartProcessAsync(CancellationToken cancellationToken);
    }
}