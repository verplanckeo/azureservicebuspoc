using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AzureServiceBus.Messaging.Seedwork;

namespace AzureServiceBus.Messaging.User
{
    public class UserRegisteredCommand : ICommand
    {
        /// <summary>
        /// User id known on our platform
        /// </summary>
        public string UserId { get; private set;  }

        /// <summary>
        /// Platforms the user is eligible for
        /// </summary>
        public IList<SocialMediaPlatform> RegisteredPlatforms { get; private set; }

        /// <summary>
        /// Queue on which the command is to be sent
        /// </summary>
        public string Queue { get; private set; }

        private UserRegisteredCommand(string userId, IList<SocialMediaPlatform> registeredPlatforms, string queue)
        {
            UserId = userId;
            RegisteredPlatforms = registeredPlatforms;
            Queue = queue;
        }

        public static UserRegisteredCommand Create(string userId, IList<SocialMediaPlatform> registeredPlatforms, string queue = "user")
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException($"{nameof(userId)} can not be empty or null!");
            if (registeredPlatforms == null || !registeredPlatforms.Any()) throw new ArgumentNullException($"{nameof(registeredPlatforms)} is NULL or empty!");
            if (string.IsNullOrEmpty(queue)) throw new ArgumentNullException($"{nameof(queue)} can not be empty or null!");

            return new UserRegisteredCommand(userId, registeredPlatforms, queue);
        }
    }
}
