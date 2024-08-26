using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class SingleSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class SingleSubscriberHandler(ITestInterface testInterface) : PublicationHandler<SingleSubscriber>
{
    public override void Handle(SingleSubscriber notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}