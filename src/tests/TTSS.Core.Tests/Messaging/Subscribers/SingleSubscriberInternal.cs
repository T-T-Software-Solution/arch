using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

internal class SingleSubscriberInternal : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

internal class SingleSubscriberInternalHandler : PublicationHandler<SingleSubscriberInternal>
{
    private readonly ITestInterface _testInterface;

    public SingleSubscriberInternalHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(SingleSubscriberInternal notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}