using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncMultiSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class AsyncMultiSubscriberHandler1(ITestInterface testInterface) : PublicationHandlerAsync<AsyncMultiSubscriber>
{
    public override async Task HandleAsync(AsyncMultiSubscriber notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}

public class AsyncMultiSubscriberHandler2(ITestInterface testInterface) : PublicationHandlerAsync<AsyncMultiSubscriber>
{
    public override async Task HandleAsync(AsyncMultiSubscriber notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}

public class AsyncMultiSubscriberHandler3(ITestInterface testInterface) : PublicationHandlerAsync<AsyncMultiSubscriber>
{
    public override async Task HandleAsync(AsyncMultiSubscriber notification, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}