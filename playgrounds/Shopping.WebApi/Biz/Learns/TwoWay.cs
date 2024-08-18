using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Learns;

public sealed record TwoWay : IRequesting<Response>
{
    public int FirstValue { get; init; }
    public int SecondValue { get; init; }
}

internal sealed class TwoWayHandler(ILogger<TwoWayHandler> logger, ICorrelationContext context) : RequestHandler<TwoWay, Response>
{
    public override Response Handle(TwoWay request)
    {
        var result = request.FirstValue + request.SecondValue;
        var message = $"(CorrelationId: {context.CorrelationId}), (UserId: {context.CurrentUserId}), Sum of {request.FirstValue} and {request.SecondValue} is {result}";
        logger.LogInformation(message);
        return new() { Message = message };
    }
}