using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Pipelines;

public class MoreThan10PipelineBehavior<TRequest, TResponse> : PipelineBehavior<TRequest, TResponse>
    where TRequest : MoreThan10Request
{
    private readonly ITestInterface _testInterface;

    public MoreThan10PipelineBehavior(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override TResponse Handle(TRequest request, Func<TResponse> next)
    {
        if (request.Number < 10) return default!;

        _testInterface.Execute(request);
        return next();
    }
}

public class MoreThan10Request : IRequesting<MoreThan10Response>
{
    public int Number { get; set; }
}

public class MoreThan10Response
{
    public string HandlerName { get; set; }
}
public class MoreThan10Handler : RequestHandler<MoreThan10Request, MoreThan10Response>
{
    public override MoreThan10Response Handle(MoreThan10Request request)
        => new() { HandlerName = GetType().Name };
}