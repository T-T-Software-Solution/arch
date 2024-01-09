using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class SingleSubscriberCallAsyncSingleSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = new();
}

public class SingleSubscriberCallAsyncSingleSubscriberHandler : PublicationHandler<SingleSubscriberCallAsyncSingleSubscriber>
{
    private readonly IMessagingHub _messagingHub;
    private readonly ITestInterface _testInterface;

    public SingleSubscriberCallAsyncSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub)
    {
        _testInterface = testInterface;
        _messagingHub = messagingHub;
    }

    public override void Handle(SingleSubscriberCallAsyncSingleSubscriber notification)
    {
        _testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);

        var noti = new AsyncSingleSubscriber();
        var response = _messagingHub.PublishAsync(noti);
        Task.WaitAll(response);

        notification.HandlerNames.AddRange(noti.HandlerNames);
    }
}