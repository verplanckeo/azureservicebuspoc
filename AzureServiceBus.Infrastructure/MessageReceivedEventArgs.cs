using System;
using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Infrastructure
{
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The native message event args of Azure ServiceBus.
        /// </summary>
        public ProcessMessageEventArgs MessageEventArgs { get; set; }
    }
}