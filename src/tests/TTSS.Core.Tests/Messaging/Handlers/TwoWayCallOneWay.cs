namespace TTSS.Core.Messaging.Handlers;

public class TwoWayCallOneWay : IRequesting<TwoWayCallOneWayResponse>
{
    public required string Name { get; set; }
}

public class TwoWayCallOneWayResponse
{
    public TwoWayCallOneWayResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
    public required string NameFromOneWay { get; set; }
}

public class TwoWayCallOneWayHandler : RequestHandler<TwoWayCallOneWay, TwoWayCallOneWayResponse>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public TwoWayCallOneWayHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override TwoWayCallOneWayResponse Handle(TwoWayCallOneWay request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;

        var req = new OneWay { Name = Guid.NewGuid().ToString() };
        var response = _messagingHub.SendAsync(req);
        Task.WaitAll(response);

        return new TwoWayCallOneWayResponse(99)
        {
            NameFromOneWay = req.Name,
        };
    }
}