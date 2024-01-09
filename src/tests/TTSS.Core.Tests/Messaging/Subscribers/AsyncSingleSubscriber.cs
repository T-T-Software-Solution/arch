using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncSingleSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class AsyncSingleSubscriberHandler : PublicationHandlerAsync<AsyncSingleSubscriber>
{
    private readonly ITestInterface _testInterface;

    public AsyncSingleSubscriberHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task HandleAsync(AsyncSingleSubscriber notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}