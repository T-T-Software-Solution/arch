using TTSS.Core.Messaging.Subscribers;

namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayCallSingleSubscriber : IRequesting
{
    public required string Name { get; set; }
    public List<string> NotificationHandlerNames { get; set; } = [];
}

public class AsyncOneWayCallSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandlerAsync<AsyncOneWayCallSingleSubscriber>
{
    public override async Task HandleAsync(AsyncOneWayCallSingleSubscriber request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var noti = new SingleSubscriber();
        await messagingHub.PublishAsync(noti, cancellationToken);

        request.NotificationHandlerNames.AddRange(noti.HandlerNames);
    }
}