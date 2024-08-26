namespace TTSS.Core.Messaging.Handlers;

public class TwoWayInternal : IRequesting<TwoWayResponse>
{
    public required string Name { get; set; }
}

public class TwoWayInternalHandler(ITestInterface testInterface) : RequestHandler<TwoWayInternal, TwoWayResponse>
{
    public override TwoWayResponse Handle(TwoWayInternal request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;
        return new TwoWayResponse(99);
    }
}