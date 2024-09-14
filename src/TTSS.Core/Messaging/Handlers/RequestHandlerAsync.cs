namespace TTSS.Core.Messaging.Handlers;

/// <summary>
/// Defines an asynchronous handler for a request.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
public abstract class RequestHandlerAsync<TRequest> : MediatR.IRequestHandler<TRequest>
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
        => HandleAsync(request, cancellationToken);

    /// <summary>
    /// Handles a request.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public abstract Task HandleAsync(TRequest request, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// Defines an asynchronous handler for a request with a response.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public abstract class RequestHandlerAsync<TRequest, TResponse> : MediatR.IRequestHandler<TRequest, TResponse>
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
        => HandleAsync(request, cancellationToken);

    /// <summary>
    /// Handles a request.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);

    #endregion
}