using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Learns;

public sealed record OneWay : IRequesting
{
    public required string Input { get; init; }
}

internal sealed class OneWayHandler(ILogger<OneWayHandler> logger, ICorrelationContext context) : RequestHandler<OneWay>
{
    public override void Handle(OneWay request)
        => logger.LogInformation($"(CorrelationId: {context.CorrelationId}), (UserId: {context.CurrentUserId}), Received: {request.Input}");
}