namespace TTSS.Core.Messaging.Handlers;

public class TwoWay : IRequesting<TwoWayResponse>
{
    public required string Name { get; set; }
}

public class TwoWayResponse
{
    public TwoWayResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
}

public class TwoWayHandler : RequestHandler<TwoWay, TwoWayResponse>
{
    private readonly ITestInterface _testInterface;

    public TwoWayHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override TwoWayResponse Handle(TwoWay request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;
        return new TwoWayResponse(99);
    }
}