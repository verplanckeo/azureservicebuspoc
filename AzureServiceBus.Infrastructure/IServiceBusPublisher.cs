using System.Threading;
using System.Threading.Tasks;
using AzureServiceBus.Messaging.Seedwork;

namespace AzureServiceBus.Infrastructure
{
    public interface IServiceBusPublisher
    {
        /// <summary>
        /// Send a command to a given queue.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendAsync(ICommand command, CancellationToken cancellationToken);

        /// <summary>
        /// Publish an event on ASB.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync(IEvent evt, CancellationToken cancellationToken);
    }
}