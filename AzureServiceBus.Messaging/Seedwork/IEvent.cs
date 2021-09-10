namespace AzureServiceBus.Messaging.Seedwork
{
    /*
     * An event is a lightweight notification of a condition or a state change. The publisher of the event has no expectation about how the event is handled.
     * The consumer of the event decides what to do with the notification. Events can be discrete units or part of a series.
     * Discrete events report state change and are actionable. To take the next step, the consumer only needs to know that something happened.
     * The event data has information about what happened but doesn't have the data that triggered the event. For example, an event notifies consumers that a file was created.
     * It may have general information about the file, but it doesn't have the file itself. Discrete events are ideal for serverless solutions that need to scale.
     */

    /// <summary>
    /// An event to broadcast to all interested handlers something has happened.
    /// </summary>
    public interface IEvent : IMessage
    {
        string Topic { get; }
    }
}