using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

internal class AsyncSingleSubscriberInternal : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

internal class AsyncSingleSubscriberInternalHandler : PublicationHandlerAsync<AsyncSingleSubscriberInternal>
{
    private readonly ITestInterface _testInterface;

    public AsyncSingleSubscriberInternalHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task HandleAsync(AsyncSingleSubscriberInternal notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}