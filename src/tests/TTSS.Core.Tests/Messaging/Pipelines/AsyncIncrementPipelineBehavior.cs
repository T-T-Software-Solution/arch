using TTSS.Core.Messaging.Handlers;

namespace TTSS.Core.Messaging.Pipelines;

public class AsyncIncrementPipelineBehavior<TRequest, TResponse> : PipelineBehaviorAsync<TRequest, TResponse>
    where TRequest : AsyncIncrementRequest
{
    private readonly ITestInterface _testInterface;

    public AsyncIncrementPipelineBehavior(ITestInterface testInterface)
        => _testInterface = testInterface;

    public override async Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        await _testInterface.ExecuteAsync(request, cancellationToken);
        request.Number++;
        return await next();
    }
}

public class AsyncIncrementRequest : IRequesting<IncrementResponse>
{
    public int Number { get; set; }
}

public class AsyncIncrementHandler : RequestHandlerAsync<AsyncIncrementRequest, IncrementResponse>
{
    public override Task<IncrementResponse> HandleAsync(AsyncIncrementRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult<IncrementResponse>(new() { HandlerName = GetType().Name });
}