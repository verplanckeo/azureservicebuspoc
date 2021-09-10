namespace AzureServiceBus.Infrastructure
{
    public class Constants
    {
        public class ServiceBus
        {
            public const string TopicName = "artificitopic";
            public const string ServiceBusKey = "AzureWebJobsServiceBus";
        }
        public class MessageHeaders
        {
            public const string MessageTypeKey = "MessageType";

            public const string MessageErrorKey = "Error";
        }

        public class ErrorMessages
        {
            public const string HandlerIsAlreadyAssigned = "Handler is already assigned";
            public const string HandlerHasNotBeenAssigned = "Handler is not assigned";
        }
    }
}