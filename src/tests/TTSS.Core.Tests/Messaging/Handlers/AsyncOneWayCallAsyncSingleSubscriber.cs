using TTSS.Core.Messaging.Subscribers;

namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayCallAsyncSingleSubscriber : IRequesting
{
    public required string Name { get; set; }
    public List<string> NotificationHandlerNames { get; set; } = new();
}

public class AsyncOneWayCallAsyncSingleSubscriberHandler : RequestHandlerAsync<AsyncOneWayCallAsyncSingleSubscriber>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncOneWayCallAsyncSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task HandleAsync(AsyncOneWayCallAsyncSingleSubscriber request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var noti = new AsyncSingleSubscriber();
        await _messagingHub.PublishAsync(noti);

        request.NotificationHandlerNames.AddRange(noti.HandlerNames);
    }
}