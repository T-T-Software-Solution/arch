using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

internal class AsyncSingleSubscriberInternal : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

internal class AsyncSingleSubscriberInternalHandler(ITestInterface testInterface) : PublicationHandlerAsync<AsyncSingleSubscriberInternal>
{
    public override async Task HandleAsync(AsyncSingleSubscriberInternal notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}