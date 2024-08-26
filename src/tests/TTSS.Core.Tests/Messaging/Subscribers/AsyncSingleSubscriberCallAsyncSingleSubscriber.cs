using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncSingleSubscriberCallAsyncSingleSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class AsyncSingleSubscriberCallAsyncSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub) : PublicationHandlerAsync<AsyncSingleSubscriberCallAsyncSingleSubscriber>
{
    public override async Task HandleAsync(AsyncSingleSubscriberCallAsyncSingleSubscriber notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);

        var noti = new AsyncSingleSubscriber();
        await messagingHub.PublishAsync(noti, cancellationToken);

        notification.HandlerNames.AddRange(noti.HandlerNames);
    }
}