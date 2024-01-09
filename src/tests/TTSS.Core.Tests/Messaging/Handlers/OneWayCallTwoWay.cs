namespace TTSS.Core.Messaging.Handlers;

public class OneWayCallTwoWay : IRequesting
{
    public required string Name { get; set; }
    public int ValueFromTwoWay { get; set; }
}

public class OneWayCallTwoWayHandler : RequestHandler<OneWayCallTwoWay>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public OneWayCallTwoWayHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override void Handle(OneWayCallTwoWay request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;

        var response = _messagingHub.SendAsync(new TwoWay { Name = Guid.NewGuid().ToString() });
        Task.WaitAll(response);

        request.ValueFromTwoWay = response.Result.Value;
    }
}