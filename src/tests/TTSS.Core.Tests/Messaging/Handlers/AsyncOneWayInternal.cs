namespace TTSS.Core.Messaging.Handlers;

internal class AsyncOneWayInternal : IRequesting
{
    public required string Name { get; set; }
}

internal class AsyncOneWayInternalHandler : RequestHandlerAsync<AsyncOneWayInternal>
{
    private readonly ITestInterface _testInterface;

    public AsyncOneWayInternalHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task HandleAsync(AsyncOneWayInternal request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;
    }
}