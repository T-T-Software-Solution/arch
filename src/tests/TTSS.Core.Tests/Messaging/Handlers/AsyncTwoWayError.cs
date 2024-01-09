namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWayError : IRequesting<AsyncTwoWayErrorResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayErrorResponse
{
    public AsyncTwoWayErrorResponse(int value)
    {
        Value = value;
    }

    public int Value { get; }
}

public class AsyncTwoWayErrorHandler : RequestHandler<AsyncTwoWayError, AsyncTwoWayErrorResponse>
{
    private readonly ITestInterface _testInterface;

    public AsyncTwoWayErrorHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override AsyncTwoWayErrorResponse Handle(AsyncTwoWayError request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;
        throw new InvalidOperationException();
    }
}