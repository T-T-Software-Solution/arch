﻿namespace TTSS.Core.Messaging.Handlers;

/// <summary>
/// Defines an asynchronous handler for a remote request.
/// </summary>
public abstract class RemoteRequestHandlerAsync;

/// <summary>
/// Defines an asynchronous handler for a remote request.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
public abstract class RemoteRequestHandlerAsync<TRequest> : RemoteRequestHandlerAsync,
    MassTransit.IConsumer<TRequest>
    where TRequest : class, IRemoteRequesting
{
    #region Properties

    /// <summary>
    /// Consumer context.
    /// </summary>
    protected MassTransit.ConsumeContext<TRequest> RemoteContext { get; private set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Defines a class that is a consumer of a message. The message is wrapped in an IConsumeContext
    /// interface to allow access to details surrounding the inbound message, including headers.
    /// </summary>
    Task MassTransit.IConsumer<TRequest>.Consume(MassTransit.ConsumeContext<TRequest> context)
    {
        RemoteContext = context;
        return HandleAsync(context.Message, context.CancellationToken);
    }

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
/// Defines an asynchronous handler for a remote request with a response.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public abstract class RemoteRequestHandlerAsync<TRequest, TResponse> : RemoteRequestHandlerAsync,
    MassTransit.IConsumer<TRequest>
    where TRequest : class, IRemoteRequesting<TResponse>
    where TResponse : class
{
    #region Properties

    /// <summary>
    /// Consumer context.
    /// </summary>
    protected MassTransit.ConsumeContext<TRequest> RemoteContext { get; private set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Defines a class that is a consumer of a message. The message is wrapped in an IConsumeContext
    /// interface to allow access to details surrounding the inbound message, including headers.
    /// </summary>
    async Task MassTransit.IConsumer<TRequest>.Consume(MassTransit.ConsumeContext<TRequest> context)
    {
        RemoteContext = context;
        var response = await HandleAsync(context.Message, context.CancellationToken);
        await context.RespondAsync(response);
    }

    /// <summary>
    /// Handles a request.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);

    #endregion
}