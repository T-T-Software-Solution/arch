using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class ChainPublishNotification : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class ChainPublishNotificationHandler : PublicationHandler<ChainPublishNotification>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public ChainPublishNotificationHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override void Handle(ChainPublishNotification notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);

        var noti = new MultiSubscriber();
        var response = _messagingHub.PublishAsync(noti);
        Task.WaitAll(response);

        notification.HandlerNames.AddRange(noti.HandlerNames);
    }
}