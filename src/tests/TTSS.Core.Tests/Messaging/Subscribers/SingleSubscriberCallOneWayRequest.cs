using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class SingleSubscriberCallOneWayRequest : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class SingleSubscriberCallOneWayRequestHandler : PublicationHandler<SingleSubscriberCallOneWayRequest>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public SingleSubscriberCallOneWayRequestHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override void Handle(SingleSubscriberCallOneWayRequest notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);

        var request = new OneWay { Name = Guid.NewGuid().ToString() };
        var response = _messagingHub.SendAsync(request);
        Task.WaitAll(response);

        notification.HandlerNames.Add(request.Name);
    }
}