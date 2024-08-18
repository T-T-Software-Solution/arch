namespace TTSS.Core.Messaging.Handlers;

public class OneWayError : IRequesting
{
    public required string Name { get; set; }
}

public class OneWayErrorHandler(ITestInterface testInterface) : RequestHandler<OneWayError>
{
    public override void Handle(OneWayError request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;
        throw new InvalidOperationException();
    }
}
