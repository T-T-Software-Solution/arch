namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWay : IRequesting<AsyncTwoWayResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayResponse
{
    public AsyncTwoWayResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
}

public class AsyncTwoWayHandler : RequestHandlerAsync<AsyncTwoWay, AsyncTwoWayResponse>
{
    private readonly ITestInterface _testInterface;

    public AsyncTwoWayHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task<AsyncTwoWayResponse> HandleAsync(AsyncTwoWay request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;
        return new AsyncTwoWayResponse(999);
    }
}