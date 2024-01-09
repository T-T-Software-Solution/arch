namespace TTSS.Core.Messaging.Handlers;

public class TwoWayError : IRequesting<TwoWayErrorResponse>
{
    public required string Name { get; set; }
}

public class TwoWayErrorResponse
{
    public TwoWayErrorResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
}

public class TwoWayErrorHandler : RequestHandler<TwoWayError, TwoWayErrorResponse>
{
    private readonly ITestInterface _testInterface;

    public TwoWayErrorHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override TwoWayErrorResponse Handle(TwoWayError request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;
        throw new InvalidOperationException();
    }
}