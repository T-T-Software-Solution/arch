using TTSS.Core.Messaging.Subscribers;

namespace TTSS.Core.Messaging.Handlers;

public class OneWayCallSingleSubscriber : IRequesting
{
    public required string Name { get; set; }
    public List<string> NotificationHandlerNames { get; set; } = new();
}

public class OneWayCallSingleSubscriberHandler : RequestHandler<OneWayCallSingleSubscriber>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public OneWayCallSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override void Handle(OneWayCallSingleSubscriber request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;

        var noti = new SingleSubscriber();
        var response = _messagingHub.PublishAsync(noti);
        Task.WaitAll(response);

        request.NotificationHandlerNames.AddRange(noti.HandlerNames);
    }
}