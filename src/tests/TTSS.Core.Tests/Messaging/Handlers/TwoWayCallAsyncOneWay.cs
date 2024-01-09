namespace TTSS.Core.Messaging.Handlers;

public class TwoWayCallAsyncOneWay : IRequesting<TwoWayCallOneWayResponse>
{
    public required string Name { get; set; }
}

public class TwoWayCallAsyncOneWayHandler : RequestHandler<TwoWayCallAsyncOneWay, TwoWayCallOneWayResponse>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public TwoWayCallAsyncOneWayHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override TwoWayCallOneWayResponse Handle(TwoWayCallAsyncOneWay request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;

        var req = new AsyncOneWay { Name = Guid.NewGuid().ToString() };
        var response = _messagingHub.SendAsync(req);
        Task.WaitAll(response);

        return new TwoWayCallOneWayResponse(99)
        {
            NameFromOneWay = req.Name,
        };
    }
}