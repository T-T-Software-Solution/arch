using Sample12.Basic.WebApp.ChainRequest.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample12.Basic.WebApp.ChainRequest.Handlers;

internal sealed class AddRequestHandler : RequestHandler<AddRequest, double>
{
    public override double Handle(AddRequest request)
    {
        return request.Operand1 + request.Operand2;
    }
}