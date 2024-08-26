namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWay : IRequesting
{
    public required string Name { get; set; }
}

public class AsyncOneWayHandler(ITestInterface testInterface) : RequestHandlerAsync<AsyncOneWay>
{
    public override async Task HandleAsync(AsyncOneWay request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;
    }
}