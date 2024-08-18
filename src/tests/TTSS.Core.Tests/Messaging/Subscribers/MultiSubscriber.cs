using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class MultiSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class MultiSubscriberHandler1(ITestInterface testInterface) : PublicationHandler<MultiSubscriber>
{
    public override void Handle(MultiSubscriber notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}

public class MultiSubscriberHandler2(ITestInterface testInterface) : PublicationHandler<MultiSubscriber>
{
    public override void Handle(MultiSubscriber notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}

public class MultiSubscriberHandler3(ITestInterface testInterface) : PublicationHandler<MultiSubscriber>
{
    public override void Handle(MultiSubscriber notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}