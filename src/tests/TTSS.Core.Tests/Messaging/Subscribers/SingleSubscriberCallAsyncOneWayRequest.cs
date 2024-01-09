using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class SingleSubscriberCallAsyncOneWayRequest : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class SingleSubscriberCallAsyncOneWayRequestHandler : PublicationHandler<SingleSubscriberCallAsyncOneWayRequest>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public SingleSubscriberCallAsyncOneWayRequestHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override void Handle(SingleSubscriberCallAsyncOneWayRequest notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);

        var request = new AsyncOneWay { Name = Guid.NewGuid().ToString() };
        var response = _messagingHub.SendAsync(request);
        Task.WaitAll(response);

        notification.HandlerNames.Add(request.Name);
    }
}