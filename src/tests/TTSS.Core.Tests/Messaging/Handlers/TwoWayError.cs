namespace TTSS.Core.Messaging.Handlers;

public class TwoWayError : IRequesting<TwoWayErrorResponse>
{
    public required string Name { get; set; }
}

public class TwoWayErrorResponse(int value)
{
    public int Value { get; } = value;
}

public class TwoWayErrorHandler(ITestInterface testInterface) : RequestHandler<TwoWayError, TwoWayErrorResponse>
{
    public override TwoWayErrorResponse Handle(TwoWayError request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;
        throw new InvalidOperationException();
    }
}