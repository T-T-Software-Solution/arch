namespace TTSS.Core.Messaging.Handlers;

internal class OneWayInternal : IRequesting
{
    public required string Name { get; set; }
}

internal class OneWayInternalHandler : RequestHandler<OneWayInternal>
{
    private readonly ITestInterface _testInterface;

    public OneWayInternalHandler(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override void Handle(OneWayInternal request)
    {
        _testInterface.Execute(request);
        request.Name = GetType().Name;
    }
}