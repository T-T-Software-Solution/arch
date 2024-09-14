using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Learns;

public sealed record GetCorrelationContext() : IRequesting<ICorrelationContext>;

file sealed class Handler(ICorrelationContext context) : RequestHandler<GetCorrelationContext, ICorrelationContext>
{
    public override ICorrelationContext Handle(GetCorrelationContext request)
        => context;
}