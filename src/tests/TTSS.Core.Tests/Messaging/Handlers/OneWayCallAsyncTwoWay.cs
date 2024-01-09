namespace TTSS.Core.Messaging.Handlers;

public class OneWayCallAsyncTwoWay : IRequesting
{
    public required string Name { get; set; }
    public int ValueFromTwoWay { get; set; }
}

public class OneWayCallAsyncTwoWayHandler : RequestHandler<OneWayCallAsyncTwoWay>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public OneWayCallAsyncTwoWayHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override void Handle(OneWayCallAsyncTwoWay request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;

        var response = _messagingHub.SendAsync(new AsyncTwoWay { Name = Guid.NewGuid().ToString() });
        Task.WaitAll(response);

        request.ValueFromTwoWay = response.Result.Value;
    }
}