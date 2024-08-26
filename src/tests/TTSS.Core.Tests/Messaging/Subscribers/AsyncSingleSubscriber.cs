using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncSingleSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class AsyncSingleSubscriberHandler(ITestInterface testInterface) : PublicationHandlerAsync<AsyncSingleSubscriber>
{
    public override async Task HandleAsync(AsyncSingleSubscriber notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}