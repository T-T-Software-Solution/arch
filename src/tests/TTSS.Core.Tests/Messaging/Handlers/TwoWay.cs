namespace TTSS.Core.Messaging.Handlers;

public class TwoWay : IRequesting<TwoWayResponse>
{
    public required string Name { get; set; }
}

public class TwoWayResponse(int value)
{
    public int Value { get; } = value;
}

public class TwoWayHandler(ITestInterface testInterface) : RequestHandler<TwoWay, TwoWayResponse>
{
    public override TwoWayResponse Handle(TwoWay request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;
        return new TwoWayResponse(99);
    }
}