using TTSS.Core.Messaging.Subscribers;

namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayCallAsyncSingleSubscriber : IRequesting
{
    public required string Name { get; set; }
    public List<string> NotificationHandlerNames { get; set; } = [];
}

public class AsyncOneWayCallAsyncSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandlerAsync<AsyncOneWayCallAsyncSingleSubscriber>
{
    public override async Task HandleAsync(AsyncOneWayCallAsyncSingleSubscriber request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var noti = new AsyncSingleSubscriber();
        await messagingHub.PublishAsync(noti, cancellationToken);

        request.NotificationHandlerNames.AddRange(noti.HandlerNames);
    }
}