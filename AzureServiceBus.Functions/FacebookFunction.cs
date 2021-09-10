using System;
using System.Threading.Tasks;
using AzureServiceBus.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureServiceBus.Functions
{
    public class FacebookFunction : Startup
    {
        private readonly IServiceBusSubscriptionService _subscriptionService;

        public FacebookFunction(IServiceBusSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        
        [FunctionName("FacebookFunction")]
        public async Task Run([ServiceBusTrigger("facebookqueue", Connection = "AzureServiceBus")]object myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
    