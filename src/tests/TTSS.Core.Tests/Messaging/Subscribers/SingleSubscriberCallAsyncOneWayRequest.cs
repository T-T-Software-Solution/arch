using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

public class SingleSubscriberCallAsyncOneWayRequest : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

public class SingleSubscriberCallAsyncOneWayRequestHandler(ITestInterface testInterface, IMessagingHub messagingHub) : PublicationHandler<SingleSubscriberCallAsyncOneWayRequest>
{
    public override void Handle(SingleSubscriberCallAsyncOneWayRequest notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);

        var request = new AsyncOneWay { Name = Guid.NewGuid().ToString() };
        var response = messagingHub.SendAsync(request);
        response.Wait();

        notification.HandlerNames.Add(request.Name);
    }
}