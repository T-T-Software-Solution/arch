using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Subscribers;

internal class SingleSubscriberInternal : IPublication
{
    public List<string> HandlerNames { get; set; } = [];
}

internal class SingleSubscriberInternalHandler(ITestInterface testInterface) : PublicationHandler<SingleSubscriberInternal>
{
    public override void Handle(SingleSubscriberInternal notification)
    {
        testInterface.Execute(notification);
        notification.HandlerNames.Add(GetType().Name);
    }
}