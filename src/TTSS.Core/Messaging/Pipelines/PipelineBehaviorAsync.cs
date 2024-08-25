namespace TTSS.Core.Messaging.Pipelines;

/// <summary>
/// Pipeline behavior to surround the inner handler.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public abstract class PipelineBehaviorAsync<TRequest, TResponse> : IPipelineBehaviorBase<TRequest, TResponse>
    where TRequest : notnull
{
    #region Methods

    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> MediatR.IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, MediatR.RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => HandleAsync(request, () => next(), cancellationToken);

    /// <summary>
    /// Pipeline handler.
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable the next action in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse"/>.</returns>
    public abstract Task<TResponse> HandleAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default);

    #endregion
}