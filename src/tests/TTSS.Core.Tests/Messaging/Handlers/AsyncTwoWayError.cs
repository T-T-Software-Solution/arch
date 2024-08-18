namespace TTSS.Core.Messaging.Handlers;

public class AsyncTwoWayError : IRequesting<AsyncTwoWayErrorResponse>
{
    public required string Name { get; set; }
}

public class AsyncTwoWayErrorResponse(int value)
{
    public int Value { get; } = value;
}

public class AsyncTwoWayErrorHandler(ITestInterface testInterface) : RequestHandler<AsyncTwoWayError, AsyncTwoWayErrorResponse>
{
    public override AsyncTwoWayErrorResponse Handle(AsyncTwoWayError request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;
        throw new InvalidOperationException();
    }
}