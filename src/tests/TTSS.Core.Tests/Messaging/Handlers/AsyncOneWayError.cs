namespace TTSS.Core.Messaging.Handlers;

public class AsyncOneWayError : IRequesting
{
    public required string Name { get; set; }
}

public class AsyncOneWayErrorHandler(ITestInterface testInterface) : RequestHandler<AsyncOneWayError>
{
    public override void Handle(AsyncOneWayError request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;
        throw new InvalidOperationException();
    }
}
