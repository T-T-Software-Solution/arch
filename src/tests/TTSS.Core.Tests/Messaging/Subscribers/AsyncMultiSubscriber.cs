using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class AsyncMultiSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class AsyncMultiSubscriberHandler1 : PublicationHandlerAsync<AsyncMultiSubscriber>
{
    private readonly ITestInterface _testInterface;

    public AsyncMultiSubscriberHandler1(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task HandleAsync(AsyncMultiSubscriber notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}

public class AsyncMultiSubscriberHandler2 : PublicationHandlerAsync<AsyncMultiSubscriber>
{
    private readonly ITestInterface _testInterface;

    public AsyncMultiSubscriberHandler2(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task HandleAsync(AsyncMultiSubscriber notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}

public class AsyncMultiSubscriberHandler3 : PublicationHandlerAsync<AsyncMultiSubscriber>
{
    private readonly ITestInterface _testInterface;

    public AsyncMultiSubscriberHandler3(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task HandleAsync(AsyncMultiSubscriber notification, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(notification, cancellationToken);
        notification.HandlerNames.Add(GetType().Name);
    }
}