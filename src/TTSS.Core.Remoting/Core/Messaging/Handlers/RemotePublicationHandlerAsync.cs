namespace TTSS.Core.Messaging.Handlers;

/// <summary>
/// Base class for an asynchronous remote publication handler.
/// </summary>
public abstract class RemotePublicationHandlerAsync;

/// <summary>
/// Base class for an asynchronous remote publication handler.
/// </summary>
/// <typeparam name="TPublication">The publication type</typeparam>
public abstract class RemotePublicationHandlerAsync<TPublication> : RemotePublicationHandlerAsync,
    MassTransit.IConsumer<TPublication>
    where TPublication : class, IRemotePublication
{
    #region Properties

    /// <summary>
    /// Consumer context.
    /// </summary>
    protected MassTransit.ConsumeContext<TPublication> RemoteContext { get; private set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Defines a class that is a consumer of a message. The message is wrapped in an IConsumeContext
    /// interface to allow access to details surrounding the inbound message, including headers.
    /// </summary>
    Task MassTransit.IConsumer<TPublication>.Consume(MassTransit.ConsumeContext<TPublication> context)
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
    public abstract Task HandleAsync(TPublication request, CancellationToken cancellationToken = default);

    #endregion
}