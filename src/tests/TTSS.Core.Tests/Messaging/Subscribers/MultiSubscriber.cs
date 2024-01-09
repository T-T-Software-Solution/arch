using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class MultiSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class MultiSubscriberHandler1 : PublicationHandler<MultiSubscriber>
{
    private readonly ITestInterface _testInterface;

    public MultiSubscriberHandler1(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(MultiSubscriber notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}

public class MultiSubscriberHandler2 : PublicationHandler<MultiSubscriber>
{
    private readonly ITestInterface _testInterface;

    public MultiSubscriberHandler2(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(MultiSubscriber notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}

public class MultiSubscriberHandler3 : PublicationHandler<MultiSubscriber>
{
    private readonly ITestInterface _testInterface;

    public MultiSubscriberHandler3(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(MultiSubscriber notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}