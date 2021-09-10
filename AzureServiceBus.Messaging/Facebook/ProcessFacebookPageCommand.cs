using System;

namespace AzureServiceBus.Messaging.Facebook
{
    public class ProcessFacebookPageCommand
    {
        /// <summary>
        /// Id of the Facebook page
        /// </summary>
        public string PageId { get; set; }

        /// <summary>
        /// User id on the platform
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// From which date to start processing
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// Until which date to start processing
        /// </summary>
        public DateTime Until { get; set; }
    }
}
