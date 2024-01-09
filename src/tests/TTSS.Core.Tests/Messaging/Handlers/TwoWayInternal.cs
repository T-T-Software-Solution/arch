namespace TTSS.Core.Messaging.Handlers;

public class TwoWayInternal : IRequesting<TwoWayResponse>
{
    public required string Name { get; set; }
}

public class TwoWayInternalHandler : RequestHandler<TwoWayInternal, TwoWayResponse>
{
    private readonly ITestInterface _testInterface;

    public TwoWayInternalHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override TwoWayResponse Handle(TwoWayInternal request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;
        return new TwoWayResponse(99);
    }
}