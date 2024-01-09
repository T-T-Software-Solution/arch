namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayCallAsyncTwoWay : IRequesting
{
    public required string Name { get; set; }
    public int ValueFromTwoWay { get; set; }
}

public class AsyncOneWayCallAsyncTwoWayHandler : RequestHandlerAsync<AsyncOneWayCallAsyncTwoWay>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncOneWayCallAsyncTwoWayHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task HandleAsync(AsyncOneWayCallAsyncTwoWay request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var response = await _messagingHub.SendAsync(new AsyncTwoWay { Name = Guid.NewGuid().ToString() });

        request.ValueFromTwoWay = response.Value;
    }
}