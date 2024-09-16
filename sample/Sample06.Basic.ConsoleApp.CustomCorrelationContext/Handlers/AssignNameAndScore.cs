using Sample06.Basic.ConsoleApp.CustomCorrelationContext.Models;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample06.Basic.ConsoleApp.CustomCorrelationContext.Handlers;

public sealed record AssignNameAndScore(string Name) : IRequesting<DemoContext>;

file sealed class Handler(ICorrelationContext context, IMessagingHub hub) : RequestHandlerAsync<AssignNameAndScore, DemoContext>
{
    public override async Task<DemoContext> HandleAsync(AssignNameAndScore request, CancellationToken cancellationToken = default)
    {
        if (context is not DemoContext customCtx)
        {
            throw new InvalidOperationException("Invalid context type.");
        }

        await hub.SendAsync(new AssignDogAndContextBag(), cancellationToken);

        customCtx.Name = request.Name;
        customCtx.Value = context.ContextBag["MyKey"].ToString();
        customCtx.Score = new Random().Next(1, 100);

        return customCtx;
    }
}
