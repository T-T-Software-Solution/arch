namespace TTSS.Core.Messaging.Handlers;

public class OneWayCallAsyncTwoWay : IRequesting
{
    public required string Name { get; set; }
    public int ValueFromTwoWay { get; set; }
}

public class OneWayCallAsyncTwoWayHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandler<OneWayCallAsyncTwoWay>
{
    public override void Handle(OneWayCallAsyncTwoWay request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;

        var response = messagingHub.SendAsync(new AsyncTwoWay { Name = Guid.NewGuid().ToString() });
        response.Wait();

        request.ValueFromTwoWay = response.Result.Value;
    }
}