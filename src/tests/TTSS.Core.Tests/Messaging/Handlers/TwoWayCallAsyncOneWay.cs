namespace TTSS.Core.Messaging.Handlers;

public class TwoWayCallAsyncOneWay : IRequesting<TwoWayCallOneWayResponse>
{
    public required string Name { get; set; }
}

public class TwoWayCallAsyncOneWayHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandler<TwoWayCallAsyncOneWay, TwoWayCallOneWayResponse>
{
    public override TwoWayCallOneWayResponse Handle(TwoWayCallAsyncOneWay request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;

        var req = new AsyncOneWay { Name = Guid.NewGuid().ToString() };
        var response = messagingHub.SendAsync(req);
        response.Wait();

        return new TwoWayCallOneWayResponse(99)
        {
            NameFromOneWay = req.Name,
        };
    }
}