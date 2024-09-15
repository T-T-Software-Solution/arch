using MassTransit;
using TTSS.Core.Models;

namespace TTSS.Core.Messaging.Pipelines;

/// <summary>
/// Pipeline filter to set the correlation context.
/// </summary>
/// <typeparam name="TRemoteRequesting">Remote requesting type</typeparam>
/// <param name="correlationContext">Correlation context</param>
public class CorrelationPipelineFilter<TRemoteRequesting>(ICorrelationContext correlationContext)
    : IFilter<ConsumeContext<TRemoteRequesting>>
    where TRemoteRequesting : class
{
    #region Methods

    Task IFilter<ConsumeContext<TRemoteRequesting>>.Send(ConsumeContext<TRemoteRequesting> context, IPipe<ConsumeContext<TRemoteRequesting>> next)
    {
        if (string.IsNullOrWhiteSpace(correlationContext.CorrelationId)
            && correlationContext is ISetterCorrelationContext setter)
        {
            var correlationId = context.CorrelationId.HasValue
                ? context.CorrelationId.Value.ToString()
                : Guid.NewGuid().ToString();
            setter.SetCorrelationId(correlationId);
        }

        return next.Send(context);
    }

    void IProbeSite.Probe(ProbeContext context)
        => context.CreateFilterScope("correlationIdMiddleware");

    #endregion
}