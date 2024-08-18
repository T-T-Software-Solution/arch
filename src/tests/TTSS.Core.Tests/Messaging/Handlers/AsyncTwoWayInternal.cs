namespace TTSS.Core.Messaging.Handlers;

internal class AsyncTwoWayInternal : IRequesting<AsyncTwoWayInternalResponse>
{
    public required string Name { get; set; }
}

internal class AsyncTwoWayInternalResponse(int value)
{
    public int Value { get; } = value;
}

internal class AsyncTwoWayInternalHandler(ITestInterface testInterface) : RequestHandlerAsync<AsyncTwoWayInternal, AsyncTwoWayInternalResponse>
{
    public override async Task<AsyncTwoWayInternalResponse> HandleAsync(AsyncTwoWayInternal request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;
        return new AsyncTwoWayInternalResponse(999);
    }
}