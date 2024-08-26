using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class ChainPublishNotification : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class ChainPublishNotificationHandler(ITestInterface testInterface, IMessagingHub messagingHub) : PublicationHandler<ChainPublishNotification>
{
    public override void Handle(ChainPublishNotification notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);

        var noti = new MultiSubscriber();
        var response = messagingHub.PublishAsync(noti);
        response.Wait();

        notification.HandlerNames.AddRange(noti.HandlerNames);
    }
}