namespace TTSS.Core.Messaging.Handlers;

/// <summary>
/// Base class for an asynchronous publication handler.
/// </summary>
/// <typeparam name="TPublication">The publication type</typeparam>
public abstract class PublicationHandlerAsync<TPublication> : MediatR.INotificationHandler<TPublication>
    where TPublication : IPublication
{
    #region Methods

    /// <summary>
    /// Handles a notification.
    /// </summary>
    /// <param name="notification">The notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MediatR.INotificationHandler<TPublication>.Handle(TPublication notification, CancellationToken cancellationToken)
        => HandleAsync(notification, cancellationToken);

    /// <summary>
    /// Handles a publication.
    /// </summary>
    /// <param name="publication">The publication</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public abstract Task HandleAsync(TPublication publication, CancellationToken cancellationToken);

    #endregion
}