namespace TTSS.Core.Messaging.Handlers;

public class TwoWayCallOneWay : IRequesting<TwoWayCallOneWayResponse>
{
    public required string Name { get; set; }
}

public class TwoWayCallOneWayResponse(int value)
{
    public int Value { get; } = value;
    public required string NameFromOneWay { get; set; }
}

public class TwoWayCallOneWayHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandler<TwoWayCallOneWay, TwoWayCallOneWayResponse>
{
    public override TwoWayCallOneWayResponse Handle(TwoWayCallOneWay request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;

        var req = new OneWay { Name = Guid.NewGuid().ToString() };
        var response = messagingHub.SendAsync(req);
        response.Wait();

        return new TwoWayCallOneWayResponse(99)
        {
            NameFromOneWay = req.Name,
        };
    }
}