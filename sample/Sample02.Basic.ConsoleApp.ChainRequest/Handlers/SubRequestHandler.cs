using Sample02.Basic.ConsoleApp.ChainRequest.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample02.Basic.ConsoleApp.ChainRequest.Handlers;

internal sealed class SubRequestHandler : RequestHandler<SubRequest, double>
{
    public override double Handle(SubRequest request)
    {
        return request.Operand1 - request.Operand2;
    }
}