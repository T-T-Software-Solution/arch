using Sample02.Basic.ConsoleApp.ChainRequest.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample02.Basic.ConsoleApp.ChainRequest.Handlers;

internal sealed class AddRequestHandler : RequestHandler<AddRequest, double>
{
    public override double Handle(AddRequest request)
    {
        return request.Operand1 + request.Operand2;
    }
}