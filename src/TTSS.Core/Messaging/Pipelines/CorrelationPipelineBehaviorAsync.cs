using TTSS.Core.Models;

namespace TTSS.Core.Messaging.Pipelines;

/// <summary>
/// Pipeline behavior to set the correlation context.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
/// <param name="context">The correlation context</param>
internal class CorrelationPipelineBehaviorAsync<TRequest, TResponse>(ICorrelationContext context) : PipelineBehaviorAsync<TRequest, TResponse>
    where TRequest : notnull
{
    #region Methods

    /// <summary>
    /// Pipeline handler.
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable the next action in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse"/>.</returns>
    public override Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        if (context?.CorrelationId is null && context is ISetterCorrelationContext setter)
        {
            setter.SetCorrelationId(Guid.NewGuid().ToString());
        }

        return next();
    }

    #endregion
}