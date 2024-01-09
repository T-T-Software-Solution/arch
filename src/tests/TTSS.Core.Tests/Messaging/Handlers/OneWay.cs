namespace TTSS.Core.Messaging.Handlers;

public class OneWay : IRequesting
{
    public required string Name { get; set; }
}

public class OneWayHandler : RequestHandler<OneWay>
{
    private readonly ITestInterface _testInterface;

    public OneWayHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(OneWay request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;
    }
}