namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWayCallOneWay : IRequesting<AsyncTwoWayCallOneWayResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayCallOneWayResponse
{
    public AsyncTwoWayCallOneWayResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
    public required string NameFromOneWay { get; set; }
}

public class AsyncTwoWayCallOneWayHandler : RequestHandlerAsync<AsyncTwoWayCallOneWay, AsyncTwoWayCallOneWayResponse>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncTwoWayCallOneWayHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task<AsyncTwoWayCallOneWayResponse> HandleAsync(AsyncTwoWayCallOneWay request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var req = new OneWay { Name = Guid.NewGuid().ToString() };
        await _messagingHub.SendAsync(req);

        return new AsyncTwoWayCallOneWayResponse(999)
        {
            NameFromOneWay = req.Name,
        };
    }
}