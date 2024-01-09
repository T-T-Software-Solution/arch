namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayCallTwoWay : IRequesting
{
    public required string Name { get; set; }
    public int ValueFromTwoWay { get; set; }
}

public class AsyncOneWayCallTwoWayHandler : RequestHandlerAsync<AsyncOneWayCallTwoWay>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public AsyncOneWayCallTwoWayHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override async Task HandleAsync(AsyncOneWayCallTwoWay request, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var response = await _messagingHub.SendAsync(new TwoWay { Name = Guid.NewGuid().ToString() });

        request.ValueFromTwoWay = response.Value;
    }
}