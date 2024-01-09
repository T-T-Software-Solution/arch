namespace TTSS.Core.Messaging.Subscribers;

public class AsyncNoneSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}