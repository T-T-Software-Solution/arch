namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayError : IRequesting
{
    public required string Name { get; set; }
}

public class AsyncOneWayErrorHandler : RequestHandler<AsyncOneWayError>
{
    private readonly ITestInterface _testInterface;

    public AsyncOneWayErrorHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(AsyncOneWayError request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;
        throw new InvalidOperationException();
    }
}
