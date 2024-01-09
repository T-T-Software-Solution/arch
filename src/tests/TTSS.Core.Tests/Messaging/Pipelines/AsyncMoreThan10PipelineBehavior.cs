using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Pipelines;

public class AsyncMoreThan10PipelineBehavior<TRequest, TResponse> : PipelineBehaviorAsync<TRequest, TResponse>
    where TRequest : AsyncMoreThan10Request
{
    private readonly ITestInterface _testInterface;

    public AsyncMoreThan10PipelineBehavior(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        if (request.Number < 10) return default!;

        await _testInterface.ExecuteAsync(request, cancellationToken);
        return await next();
    }
}

public class AsyncMoreThan10Request : IRequesting<MoreThan10Response>
{
    public int Number { get; set; }
}

public class AsyncMoreThan10Handler : RequestHandlerAsync<AsyncMoreThan10Request, MoreThan10Response>
{
    public override Task<MoreThan10Response> HandleAsync(AsyncMoreThan10Request request, CancellationToken cancellationToken = default)
        => Task.FromResult<MoreThan10Response>(new() { HandlerName = GetType().Name });
}