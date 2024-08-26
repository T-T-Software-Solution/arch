namespace TTSS.Core.Messaging.Handlers;

internal class AsyncOneWayInternal : IRequesting
{
    public required string Name { get; set; }
}

internal class AsyncOneWayInternalHandler(ITestInterface testInterface) : RequestHandlerAsync<AsyncOneWayInternal>
{
    public override async Task HandleAsync(AsyncOneWayInternal request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;
    }
}