using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Services;

namespace TTSS.Core.Messaging;

/// <summary>
/// Defines a remote messaging hub to encapsulate request/response and publishing interaction patterns.
/// </summary>
/// <param name="provider">Service provider</param>
internal sealed class RemoteMessagingHub(Lazy<IServiceProvider> provider) : IRemoteMessagingHub
{
    #region Properties

    private IServiceProvider ServiceProvider => provider?.Value ?? throw new InvalidOperationException("The service provider is not available");

    #endregion

    #region Methods

    Task IRemoteMessagingHub.PublishAsync<TPublication>(TPublication publication, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(publication);
        var bus = ServiceProvider.GetRequiredService<IBus>();
        return bus.Publish(publication, cancellationToken);
    }

    async Task IRemoteMessagingHub.SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var bus = ServiceProvider.GetRequiredService<IBus>();
        var sender = await bus.GetSendEndpoint(new($"{bus.Address.GetBaseUri()}{typeof(TRequest).Name}"));
        await sender.Send(request, cancellationToken);
    }

    async Task<TResponse> IRemoteMessagingHub.SendAsync<TRequest, TResponse>(TRequest request,
        RequestTimeout timeout,
        Uri? destinationAddress,
        Action<SendContext<TRequest>>? callback,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);
        var bus = ServiceProvider.GetRequiredService<IBus>();
        var response = destinationAddress is null
            ? await bus.Request<TRequest, TResponse>(request, cancellationToken, timeout, callback)
            : await bus.Request<TRequest, TResponse>(destinationAddress, request, cancellationToken, timeout, callback);
        return response.Message;
    }

    #endregion
}