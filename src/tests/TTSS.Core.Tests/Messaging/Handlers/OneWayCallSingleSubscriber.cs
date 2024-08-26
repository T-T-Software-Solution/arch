using TTSS.Core.Messaging.Subscribers;

namespace TTSS.Core.Messaging.Handlers;

public class OneWayCallSingleSubscriber : IRequesting
{
    public required string Name { get; set; }
    public List<string> NotificationHandlerNames { get; set; } = [];
}

public class OneWayCallSingleSubscriberHandler(ITestInterface testInterface, IMessagingHub messagingHub) : RequestHandler<OneWayCallSingleSubscriber>
{
    public override void Handle(OneWayCallSingleSubscriber request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;

        var noti = new SingleSubscriber();
        var response = messagingHub.PublishAsync(noti);
        response.Wait();

        request.NotificationHandlerNames.AddRange(noti.HandlerNames);
    }
}