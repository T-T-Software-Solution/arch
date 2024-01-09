namespace TTSS.Core.Messaging.Handlers;

internal class AsyncTwoWayInternal : IRequesting<AsyncTwoWayInternalResponse>
{
    public required string Name { get; set; }
}

internal class AsyncTwoWayInternalResponse
{
    public AsyncTwoWayInternalResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
}

internal class AsyncTwoWayInternalHandler : RequestHandlerAsync<AsyncTwoWayInternal, AsyncTwoWayInternalResponse>
{
    private readonly ITestInterface _testInterface;

    public AsyncTwoWayInternalHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task<AsyncTwoWayInternalResponse> HandleAsync(AsyncTwoWayInternal request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;
        return new AsyncTwoWayInternalResponse(999);
    }
}