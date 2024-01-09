using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncSingleSubscriberCallAsyncOneWayRequest : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class AsyncSingleSubscriberCallAsyncOneWayRequestHandler : PublicationHandlerAsync<AsyncSingleSubscriberCallAsyncOneWayRequest>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncSingleSubscriberCallAsyncOneWayRequestHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task HandleAsync(AsyncSingleSubscriberCallAsyncOneWayRequest notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);

        var request = new AsyncOneWay { Name = Guid.NewGuid().ToString() };
        await _messagingHub.SendAsync(request);

        notification.HandlerNames.Add(request.Name);
    }
}