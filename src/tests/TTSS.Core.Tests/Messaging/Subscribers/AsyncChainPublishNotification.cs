using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncChainPublishNotification : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class AsyncChainPublishNotificationHandler : PublicationHandlerAsync<AsyncChainPublishNotification>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncChainPublishNotificationHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task HandleAsync(AsyncChainPublishNotification notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);

        var noti = new AsyncMultiSubscriber();
        await _messagingHub.PublishAsync(noti, cancellationToken);

        notification.HandlerNames.AddRange(noti.HandlerNames);
    }
}