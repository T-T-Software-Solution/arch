using TTSS.Core.Messaging.Subscribers;

namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayCallSingleSubscriber : IRequesting
{
    public required string Name { get; set; }
    public List<string> NotificationHandlerNames { get; set; } = new();
}

public class AsyncOneWayCallSingleSubscriberHandler : RequestHandlerAsync<AsyncOneWayCallSingleSubscriber>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncOneWayCallSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task HandleAsync(AsyncOneWayCallSingleSubscriber request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var noti = new SingleSubscriber();
        await _messagingHub.PublishAsync(noti);

        request.NotificationHandlerNames.AddRange(noti.HandlerNames);
    }
}