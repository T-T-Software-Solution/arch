using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncSingleSubscriberCallOneWayRequest : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class AsyncSingleSubscriberCallOneWayRequestHandler(ITestInterface testInterface, IMessagingHub messagingHub) : PublicationHandlerAsync<AsyncSingleSubscriberCallOneWayRequest>
{
    public override async Task HandleAsync(AsyncSingleSubscriberCallOneWayRequest notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);

        var request = new OneWay { Name = Guid.NewGuid().ToString() };
        await messagingHub.SendAsync(request, cancellationToken);

        notification.HandlerNames.Add(request.Name);
    }
}