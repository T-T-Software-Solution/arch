namespace TTSS.Core.Messaging.Subscribers;

public class NoneSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}