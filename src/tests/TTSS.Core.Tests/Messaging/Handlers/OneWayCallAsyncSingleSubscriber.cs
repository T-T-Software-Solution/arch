using TTSS.Core.Messaging.Subscribers;

namespace TTSS.Core.Messaging.Handlers;

public class OneWayCallAsyncSingleSubscriber : IRequesting
{
    public required string Name { get; set; }
    public List<string> NotificationHandlerNames { get; set; } = new();
}

public class OneWayCallAsyncSingleSubscriberHandler : RequestHandler<OneWayCallAsyncSingleSubscriber>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public OneWayCallAsyncSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override void Handle(OneWayCallAsyncSingleSubscriber request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;

        var noti = new AsyncSingleSubscriber();
        var response = _messagingHub.PublishAsync(noti);
        Task.WaitAll(response);

        request.NotificationHandlerNames.AddRange(noti.HandlerNames);
    }
}