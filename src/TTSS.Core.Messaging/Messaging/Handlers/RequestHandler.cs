namespace TTSS.Core.Messaging.Handlers;

/// <summary>
/// Defines a synchronous handler for a request.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
public abstract class RequestHandler<TRequest> : MediatR.IRequestHandler<TRequest>
    where TRequest : IRequesting
{
    #region Methods

    /// <summary>
    /// Handles a request.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    Task MediatR.IRequestHandler<TRequest>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        Handle(request);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles a request.
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Response from the request</returns>
    public abstract void Handle(TRequest request);

    #endregion
}

/// <summary>
/// Defines a synchronous handler for a request.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public abstract class RequestHandler<TRequest, TResponse> : MediatR.IRequestHandler<TRequest, TResponse>
    where TRequest : IRequesting<TResponse>
{
    #region Methods

    /// <summary>
    /// Handles a request.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    Task<TResponse> MediatR.IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken)
        => Task.FromResult(Handle(request));

    /// <summary>
    /// Handles a request.
    /// </summary>
    /// <param name="request">The request</param>
    /// <returns>Response from the request</returns>
    public abstract TResponse Handle(TRequest request);

    #endregion
}