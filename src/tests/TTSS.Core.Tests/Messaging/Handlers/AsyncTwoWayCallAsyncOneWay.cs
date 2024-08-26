namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWayCallAsyncOneWay : IRequesting<AsyncTwoWayCallAsyncOneWayResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayCallAsyncOneWayResponse(int value)
{
    public int Value { get; } = value;
    public required string NameFromOneWay { get; set; }
}

public class AsyncTwoWayCallAsyncOneWayHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandlerAsync<AsyncTwoWayCallAsyncOneWay, AsyncTwoWayCallAsyncOneWayResponse>
{
    public override async Task<AsyncTwoWayCallAsyncOneWayResponse> HandleAsync(AsyncTwoWayCallAsyncOneWay request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var req = new AsyncOneWay { Name = Guid.NewGuid().ToString() };
        await messagingHub.SendAsync(req, cancellationToken);

        return new AsyncTwoWayCallAsyncOneWayResponse(999)
        {
            NameFromOneWay = req.Name,
        };
    }
}