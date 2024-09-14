using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Messaging;

/// <summary>
/// Default mediator implementation relying on single and multi instance delegates for resolving handlers.
/// </summary>
/// <param name="provider">Service provider</param>
internal sealed class LocalMessagingHub(Lazy<IServiceProvider> provider) : ILocalMessagingHub
{
    #region Properties

    private IServiceProvider ServiceProvider => provider?.Value ?? throw new InvalidOperationException("The service provider is not available");

    #endregion

    #region Methods

    Task ILocalMessagingHub.PublishAsync<TPublication>(TPublication publication, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(publication);
        var mediator = ServiceProvider.GetRequiredService<MediatR.IMediator>();
        return mediator.Publish(publication, cancellationToken);
    }

    Task ILocalMessagingHub.SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var mediator = ServiceProvider.GetRequiredService<MediatR.IMediator>();
        return mediator.Send(request, cancellationToken);
    }

    Task<TResponse> ILocalMessagingHub.SendAsync<TResponse>(IRequesting<TResponse> request, CancellationToken cancellationToken)
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);
        var mediator = ServiceProvider.GetRequiredService<MediatR.IMediator>();
        return mediator.Send(request, cancellationToken);
    }

    #endregion
}