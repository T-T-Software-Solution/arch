using Sample12.Basic.WebApp.ChainRequest.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample12.Basic.WebApp.ChainRequest.Handlers;

internal sealed class SubRequestHandler : RequestHandler<SubRequest, double>
{
    public override double Handle(SubRequest request)
    {
        return request.Operand1 - request.Operand2;
    }
}