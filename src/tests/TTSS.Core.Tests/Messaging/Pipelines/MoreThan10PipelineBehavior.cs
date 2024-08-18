using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Pipelines;

public class MoreThan10PipelineBehavior<TRequest, TResponse>(ITestInterface testInterface) : PipelineBehavior<TRequest, TResponse>
    where TRequest : MoreThan10Request
{
    public override TResponse Handle(TRequest request, Func<TResponse> next)
    {
        if (request.Number < 10) return default!;

        testInterface.Execute(request);
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