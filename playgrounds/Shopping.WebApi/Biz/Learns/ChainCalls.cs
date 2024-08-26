using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Learns;

public sealed record ChainCalls : IRequesting<Response>
{
    public int FirstValue { get; init; }
    public int SecondValue { get; init; }
}

internal sealed class ChainCallHandler(IMessagingHub hub, ILogger<ChainCallHandler> logger, ICorrelationContext context) : RequestHandlerAsync<ChainCalls, Response>
{
    public override async Task<Response> HandleAsync(ChainCalls request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("(CorrelationId: {@CorrelationId}), (UserId: {@CurrentUserId})", context.CorrelationId, context.CurrentUserId);
        await hub.SendAsync(new OneWay { Input = $"Hi from {GetType().Name}" }, cancellationToken);
        return await hub.SendAsync(new TwoWay { FirstValue = request.FirstValue, SecondValue = request.SecondValue }, cancellationToken);
    }
}