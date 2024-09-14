using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Messaging;

/// <summary>
/// Default mediator implementation relying on single and multi instance delegates for resolving handlers.
/// </summary>
/// <param name="provider">Service provider</param>
internal sealed class MessagingHub(Lazy<IServiceProvider> provider) : IMessagingHub
{
    #region Properties

    private IServiceProvider ServiceProvider => provider?.Value ?? throw new InvalidOperationException("The service provider is not available");

    #endregion

    #region Methods

    /// <summary>
    /// Asynchronously send a notification to multiple handlers.
    /// </summary>
    /// <param name="publication">Notification object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the publish operation</returns>
    public Task PublishAsync<TPublication>(TPublication publication, CancellationToken cancellationToken = default)
        where TPublication : IPublication
    {
        if (publication is null)
        {
            return Task.CompletedTask;
        }

        var mediator = ServiceProvider.GetRequiredService<MediatR.IMediator>();
        return mediator.Publish(publication, cancellationToken);
    }

    /// <summary>
    /// Asynchronously send an object request to a single handler.
    /// </summary>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the type erased handler response</returns>
    public async Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequesting
    {
        if (request is null)
        {
            return;
        }

        var mediator = ServiceProvider.GetRequiredService<MediatR.IMediator>();
        await mediator.Send(request, cancellationToken);
    }

    /// <summary>
    /// Asynchronously send a request to a single handler with response.
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    public async Task<TResponse?> SendAsync<TResponse>(IRequesting<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return default;
        }

        var mediator = ServiceProvider.GetRequiredService<MediatR.IMediator>();
        return await mediator.Send(request, cancellationToken);
    }

    #endregion
}