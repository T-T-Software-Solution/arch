namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWayCallAsyncOneWay : IRequesting<AsyncTwoWayCallAsyncOneWayResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayCallAsyncOneWayResponse
{
    public AsyncTwoWayCallAsyncOneWayResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
    public required string NameFromOneWay { get; set; }
}

public class AsyncTwoWayCallAsyncOneWayHandler : RequestHandlerAsync<AsyncTwoWayCallAsyncOneWay, AsyncTwoWayCallAsyncOneWayResponse>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncTwoWayCallAsyncOneWayHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task<AsyncTwoWayCallAsyncOneWayResponse> HandleAsync(AsyncTwoWayCallAsyncOneWay request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var req = new AsyncOneWay { Name = Guid.NewGuid().ToString() };
        await _messagingHub.SendAsync(req);

        return new AsyncTwoWayCallAsyncOneWayResponse(999)
        {
            NameFromOneWay = req.Name,
        };
    }
}