namespace TTSS.Core.Messaging.Handlers;

/// <summary>
/// Base class for a publication handler.
/// </summary>
/// <typeparam name="TPublication">The publication type</typeparam>
public abstract class PublicationHandler<TPublication> : MediatR.INotificationHandler<TPublication>
    where TPublication : IPublication
{
    #region Methods

    /// <summary>
    /// Handles a notification.
    /// </summary>
    /// <param name="notification">The notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MediatR.INotificationHandler<TPublication>.Handle(TPublication notification, CancellationToken cancellationToken)
    {
        Handle(notification);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles a publication.
    /// </summary>
    /// <param name="publication">The publication</param>
    public abstract void Handle(TPublication publication);

    #endregion
}