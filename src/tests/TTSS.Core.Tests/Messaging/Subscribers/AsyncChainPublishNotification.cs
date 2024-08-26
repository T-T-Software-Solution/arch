using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncChainPublishNotification : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class AsyncChainPublishNotificationHandler(ITestInterface testInterface, IMessagingHub messagingHub) : PublicationHandlerAsync<AsyncChainPublishNotification>
{
    public override async Task HandleAsync(AsyncChainPublishNotification notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);

        var noti = new AsyncMultiSubscriber();
        await messagingHub.PublishAsync(noti, cancellationToken);

        notification.HandlerNames.AddRange(noti.HandlerNames);
    }
}