namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWay : IRequesting
{
    public required string Name { get; set; }
}

public class AsyncOneWayHandler : RequestHandlerAsync<AsyncOneWay>
{
    private readonly ITestInterface _testInterface;

    public AsyncOneWayHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task HandleAsync(AsyncOneWay request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;
    }
}