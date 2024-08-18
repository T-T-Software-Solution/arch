using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class SingleSubscriberCallAsyncSingleSubscriber : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class SingleSubscriberCallAsyncSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub) : PublicationHandler<SingleSubscriberCallAsyncSingleSubscriber>
{
    public override void Handle(SingleSubscriberCallAsyncSingleSubscriber notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);

        var noti = new AsyncSingleSubscriber();
        var response = messagingHub.PublishAsync(noti);
        response.Wait();

        notification.HandlerNames.AddRange(noti.HandlerNames);
    }
}