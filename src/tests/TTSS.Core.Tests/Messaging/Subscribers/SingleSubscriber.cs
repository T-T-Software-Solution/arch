using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class SingleSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class SingleSubscriberHandler : PublicationHandler<SingleSubscriber>
{
    private readonly ITestInterface _testInterface;

    public SingleSubscriberHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(SingleSubscriber notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}