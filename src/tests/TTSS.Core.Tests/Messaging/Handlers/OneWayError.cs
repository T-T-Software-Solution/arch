namespace TTSS.Core.Messaging.Handlers;

public class OneWayError : IRequesting
{
    public required string Name { get; set; }
}

public class OneWayErrorHandler : RequestHandler<OneWayError>
{
    private readonly ITestInterface _testInterface;

    public OneWayErrorHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(OneWayError request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;
        throw new InvalidOperationException();
    }
}
