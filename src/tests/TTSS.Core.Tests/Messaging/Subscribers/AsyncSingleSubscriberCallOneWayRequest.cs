using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncSingleSubscriberCallOneWayRequest : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class AsyncSingleSubscriberCallOneWayRequestHandler : PublicationHandlerAsync<AsyncSingleSubscriberCallOneWayRequest>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncSingleSubscriberCallOneWayRequestHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task HandleAsync(AsyncSingleSubscriberCallOneWayRequest notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);

        var request = new OneWay { Name = Guid.NewGuid().ToString() };
        await _messagingHub.SendAsync(request);

        notification.HandlerNames.Add(request.Name);
    }
}