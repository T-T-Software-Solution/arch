namespace TTSS.Core.Messaging.Handlers;

internal class OneWayInternal : IRequesting
{
    public required string Name { get; set; }
}

internal class OneWayInternalHandler(ITestInterface testInterface) : RequestHandler<OneWayInternal>
{
    public override void Handle(OneWayInternal request)
    {
        testInterface.Execute(request);
        request.Name = GetType().Name;
    }
}