using System;
using Azure.Messaging.ServiceBus;

namespace AzureServiceBus.Infrastructure
{
    public class ErrorMessageReceivedEventArgs : EventArgs
    {
        public ProcessErrorEventArgs MessageEventArgs { get; set; }
    }
}