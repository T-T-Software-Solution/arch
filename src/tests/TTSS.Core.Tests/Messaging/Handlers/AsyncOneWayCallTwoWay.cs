namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayCallTwoWay : IRequesting
{
    public required string Name { get; set; }
    public int ValueFromTwoWay { get; set; }
}

public class AsyncOneWayCallTwoWayHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandlerAsync<AsyncOneWayCallTwoWay>
{
    public override async Task HandleAsync(AsyncOneWayCallTwoWay request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var response = await messagingHub.SendAsync(new TwoWay { Name = Guid.NewGuid().ToString() }, cancellationToken);

        request.ValueFromTwoWay = response.Value;
    }
}