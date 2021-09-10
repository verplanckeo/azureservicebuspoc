using System;
using AzureServiceBus.Messaging.Seedwork;

namespace AzureServiceBus.Messaging.User
{
    public class UserRegisteredEvent : IEvent
    {
        /// <summary>
        /// User id known on our platform
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Topic to which the event is to be sent
        /// </summary>
        public string Topic { get; }

        private UserRegisteredEvent(string userId, string topic)
        {
            UserId = userId;
            Topic = topic;
        }

        public static UserRegisteredEvent Create(string userId, string topic = "topic")
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException($"{nameof(userId)} can not be empty or null!");

            return new UserRegisteredEvent(userId, topic);
        }
    }
}
