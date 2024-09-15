using MassTransit;
using TTSS.Core.Models;

namespace TTSS.Core.Messaging.Pipelines;

/// <summary>
/// Pipeline filter to set the user identity.
/// </summary>
/// <typeparam name="TRemoteRequesting">Remote requesting type</typeparam>
/// <param name="correlationContext">Correlation context</param>
public sealed class UserIdentityPipelineFilter<TRemoteRequesting>(ICorrelationContext correlationContext)
    : IFilter<ConsumeContext<TRemoteRequesting>>
    where TRemoteRequesting : class
{
    #region Methods

    Task IFilter<ConsumeContext<TRemoteRequesting>>.Send(ConsumeContext<TRemoteRequesting> context, IPipe<ConsumeContext<TRemoteRequesting>> next)
    {
        if (string.IsNullOrWhiteSpace(correlationContext.CurrentUserId)
            && correlationContext is ISetterCorrelationContext setter
            && context.Headers.TryGetHeader(RemoteMessagingHub.UserReferenceHeader, out var userId)
            && false == string.IsNullOrWhiteSpace(userId.ToString()))
        {
            setter.SetCurrentUserId(userId.ToString());
        }

        return next.Send(context);
    }

    void IProbeSite.Probe(ProbeContext context)
        => context.CreateFilterScope("UserIdentityMiddleware");

    #endregion
}