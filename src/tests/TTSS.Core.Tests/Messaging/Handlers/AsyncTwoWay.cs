namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWay : IRequesting<AsyncTwoWayResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayResponse(int value)
{
    public int Value { get; } = value;
}

public class AsyncTwoWayHandler(ITestInterface testInterface) : RequestHandlerAsync<AsyncTwoWay, AsyncTwoWayResponse>
{
    public override async Task<AsyncTwoWayResponse> HandleAsync(AsyncTwoWay request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;
        return new AsyncTwoWayResponse(999);
    }
}