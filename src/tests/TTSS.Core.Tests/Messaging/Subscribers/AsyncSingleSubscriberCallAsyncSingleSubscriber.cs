using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncSingleSubscriberCallAsyncSingleSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class AsyncSingleSubscriberCallAsyncSingleSubscriberHandler : PublicationHandlerAsync<AsyncSingleSubscriberCallAsyncSingleSubscriber>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncSingleSubscriberCallAsyncSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task HandleAsync(AsyncSingleSubscriberCallAsyncSingleSubscriber notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);

        var noti = new AsyncSingleSubscriber();
        await _messagingHub.PublishAsync(noti);

        notification.HandlerNames.AddRange(noti.HandlerNames);
    }
}