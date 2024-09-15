using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Models;
using TTSS.Core.Services;

namespace TTSS.Core.Messaging;

/// <summary>
/// Defines a remote messaging hub to encapsulate request/response and publishing interaction patterns.
/// </summary>
/// <param name="provider">Service provider</param>
/// <param name="correlationContext">Correlation context</param>
internal sealed class RemoteMessagingHub(Lazy<IServiceProvider> provider, ICorrelationContext correlationContext) : IRemoteMessagingHub
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
        var requestingType = request.GetType();
        var sender = await bus.GetSendEndpoint(new($"{bus.Address.GetBaseUri()}{requestingType.Name}"));
        await sender.Send(request, requestingType, SetCorrelationContext, cancellationToken);
    }

    async Task<TResponse> IRemoteMessagingHub.SendAsync<TRequest, TResponse>(TRequest request,
        TimeSpan timeout,
        Uri? destinationAddress,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);
        var bus = ServiceProvider.GetRequiredService<IBus>();
        var timeoutValue = timeout > TimeSpan.Zero ? timeout : RequestTimeout.Default;
        var response = destinationAddress is null
            ? await bus.Request<TRequest, TResponse>(request, cancellationToken, timeoutValue, SetCorrelationContext)
            : await bus.Request<TRequest, TResponse>(destinationAddress, request, cancellationToken, timeoutValue, SetCorrelationContext);
        return response.Message;
    }

    private void SetCorrelationContext(SendContext remoteContext)
    {
        if (false == remoteContext.InitiatorId.HasValue
            && false == string.IsNullOrWhiteSpace(correlationContext.CorrelationId))
        {
            remoteContext.InitiatorId = Guid.Parse(correlationContext.CorrelationId);
        }

        if (false == remoteContext.CorrelationId.HasValue)
        {
            remoteContext.CorrelationId = Guid.NewGuid();
        }

        if (string.IsNullOrWhiteSpace(correlationContext.CorrelationId)
            && correlationContext is ISetterCorrelationContext setter)
        {
            setter.SetCorrelationId(remoteContext.CorrelationId.ToString()!);
        }
    }

    #endregion
}