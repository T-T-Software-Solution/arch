using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class SingleSubscriberCallOneWayRequest : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class SingleSubscriberCallOneWayRequestHandler(ITestInterface testInterface, IMessagingHub messagingHub) : PublicationHandler<SingleSubscriberCallOneWayRequest>
{
    public override void Handle(SingleSubscriberCallOneWayRequest notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);

        var request = new OneWay { Name = Guid.NewGuid().ToString() };
        var response = messagingHub.SendAsync(request);
        response.Wait();

        notification.HandlerNames.Add(request.Name);
    }
}