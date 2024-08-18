using TTSS.Core.Messaging.Subscribers;

namespace TTSS.Core.Messaging.Handlers;

public class OneWayCallAsyncSingleSubscriber : IRequesting
{
    public required string Name { get; set; }
    public List<string> NotificationHandlerNames { get; set; } = [];
}

public class OneWayCallAsyncSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandler<OneWayCallAsyncSingleSubscriber>
{
    public override void Handle(OneWayCallAsyncSingleSubscriber request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;

        var noti = new AsyncSingleSubscriber();
        var response = messagingHub.PublishAsync(noti);
        response.Wait();

        request.NotificationHandlerNames.AddRange(noti.HandlerNames);
    }
}