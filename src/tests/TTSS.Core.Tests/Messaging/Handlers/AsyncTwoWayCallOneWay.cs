namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWayCallOneWay : IRequesting<AsyncTwoWayCallOneWayResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayCallOneWayResponse(int value)
{
    public int Value { get; } = value;
    public required string NameFromOneWay { get; set; }
}

public class AsyncTwoWayCallOneWayHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandlerAsync<AsyncTwoWayCallOneWay, AsyncTwoWayCallOneWayResponse>
{
    public override async Task<AsyncTwoWayCallOneWayResponse> HandleAsync(AsyncTwoWayCallOneWay request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var req = new OneWay { Name = Guid.NewGuid().ToString() };
        await messagingHub.SendAsync(req, cancellationToken);

        return new AsyncTwoWayCallOneWayResponse(999)
        {
            NameFromOneWay = req.Name,
        };
    }
}