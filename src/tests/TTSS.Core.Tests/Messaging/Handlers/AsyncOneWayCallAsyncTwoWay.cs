namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayCallAsyncTwoWay : IRequesting
{
    public required string Name { get; set; }
    public int ValueFromTwoWay { get; set; }
}

public class AsyncOneWayCallAsyncTwoWayHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandlerAsync<AsyncOneWayCallAsyncTwoWay>
{
    public override async Task HandleAsync(AsyncOneWayCallAsyncTwoWay request, CancellationToken cancellationToken = default)
    {
        await testInterface.ExecuteAsync(request, cancellationToken);
        request.Name = GetType().Name;

        var response = await messagingHub.SendAsync(new AsyncTwoWay { Name = Guid.NewGuid().ToString() }, cancellationToken);

        request.ValueFromTwoWay = response.Value;
    }
}