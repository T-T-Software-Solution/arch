using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Services;

namespace TTSS.Core.Messaging;

/// <summary>
/// Defines a remote messaging hub to encapsulate request/response and publishing interaction patterns.
/// </summary>
internal sealed class RemoteMessagingHub(Lazy<IServiceProvider> provider) : IRemoteMessagingHub
{
    #region Properties

    private IServiceProvider ServiceProvider => provider?.Value ?? throw new InvalidOperationException("The service provider is not available");

    #endregion

    #region Methods

    Task IRemoteMessagingHub.PublishAsync<TPublication>(TPublication publication, CancellationToken cancellationToken)
    {
        if (publication is null)
        {
            return Task.CompletedTask;
        }

        var bus = ServiceProvider.GetRequiredService<IBus>();
        return bus.Publish(publication, cancellationToken);
    }

    async Task IRemoteMessagingHub.SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return;
        }

        var bus = ServiceProvider.GetRequiredService<IBus>();
        var sender = await bus.GetSendEndpoint(new($"{bus.Address.GetBaseUri()}{typeof(TRequest).Name}"));
        await sender.Send(request, cancellationToken);
    }

    /// <summary>
    /// Asynchronously send a request to a single remote handler with response.
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="request">Request object</param>
    /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
    /// <param name="destinationAddress">The service address</param>
    /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
    /// <param name="cancellationToken">An optional cancellationToken for this request</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    public async Task<TResponse?> SendAsync<TRequest, TResponse>(TRequest request,
        RequestTimeout timeout = default,
        Uri? destinationAddress = default,
        Action<SendContext<TRequest>>? callback = default,
        CancellationToken cancellationToken = default)
        where TRequest : class, IRemoteRequesting<TResponse>
        where TResponse : class
    {
        if (request is null)
        {
            return default;
        }

        var bus = ServiceProvider.GetRequiredService<IBus>();
        var response = destinationAddress is null
            ? await bus.Request<TRequest, TResponse>(request, cancellationToken, timeout, callback)
            : await bus.Request<TRequest, TResponse>(destinationAddress, request, cancellationToken, timeout, callback);
        return response?.Message;
    }

    #endregion
}