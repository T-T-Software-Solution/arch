namespace TTSS.Core.Messaging.Handlers;

public class OneWay : IRequesting
{
    public required string Name { get; set; }
}

public class OneWayHandler(ITestInterface testInterface) : RequestHandler<OneWay>
{
    public override void Handle(OneWay request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;
    }
}