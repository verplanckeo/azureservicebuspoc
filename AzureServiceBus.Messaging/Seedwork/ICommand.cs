namespace AzureServiceBus.Messaging.Seedwork
{
    /*
     * A message/command is raw data produced by a service to be consumed or stored elsewhere. The message contains the data that triggered the message pipeline.
     * The publisher of the message has an expectation about how the consumer handles the message. A contract exists between the two sides. For example,
     * the publisher sends a message with the raw data, and expects the consumer to create a file from that data and send a response when the work is done.
     */

    /// <summary>
    /// A command to trigger a handler into doing a specific action.
    /// </summary>
    public interface ICommand : IMessage
    {
        string Queue { get; }
    }
}