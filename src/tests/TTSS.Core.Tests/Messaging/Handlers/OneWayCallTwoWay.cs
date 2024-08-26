namespace TTSS.Core.Messaging.Handlers;

public class OneWayCallTwoWay : IRequesting
{
    public required string Name { get; set; }
    public int ValueFromTwoWay { get; set; }
}

public class OneWayCallTwoWayHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandler<OneWayCallTwoWay>
{
    public override void Handle(OneWayCallTwoWay request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;

        var response = messagingHub.SendAsync(new TwoWay { Name = Guid.NewGuid().ToString() });
        response.Wait();

        request.ValueFromTwoWay = response.Result.Value;
    }
}