using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Pipelines;

public class IncrementPipelineBehavior<TRequest, TResponse> : PipelineBehavior<TRequest, TResponse>
    where TRequest : IncrementRequest
{
    private readonly ITestInterface _testInterface;

    public IncrementPipelineBehavior(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override TResponse Handle(TRequest request, Func<TResponse> next)
    {
        _testInterface.Execute(request);
        request.Number++;
        return next();
    }
}

public class IncrementRequest : IRequesting<IncrementResponse>
{
    public int Number { get; set; }
}

public class IncrementResponse
{
    public string HandlerName { get; set; }
}
public class IncrementHandler : RequestHandler<IncrementRequest, IncrementResponse>
{
    public override IncrementResponse Handle(IncrementRequest request)
        => new() { HandlerName = GetType().Name };
}