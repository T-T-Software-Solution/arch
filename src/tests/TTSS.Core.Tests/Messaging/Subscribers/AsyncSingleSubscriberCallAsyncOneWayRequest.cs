using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncSingleSubscriberCallAsyncOneWayRequest : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class AsyncSingleSubscriberCallAsyncOneWayRequestHandler(ITestInterface testInterface, IMessagingHub messagingHub) : PublicationHandlerAsync<AsyncSingleSubscriberCallAsyncOneWayRequest>
{
    public override async Task HandleAsync(AsyncSingleSubscriberCallAsyncOneWayRequest notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);

        var request = new AsyncOneWay { Name = Guid.NewGuid().ToString() };
        await messagingHub.SendAsync(request, cancellationToken);

        notification.HandlerNames.Add(request.Name);
    }
}