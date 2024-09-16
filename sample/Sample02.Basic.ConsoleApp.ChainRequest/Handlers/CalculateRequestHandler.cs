using Sample02.Basic.ConsoleApp.ChainRequest.Messages;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample02.Basic.ConsoleApp.ChainRequest.Handlers;

internal sealed class CalculateRequestHandler : RequestHandlerAsync<CalculateRequest, double>
{
    private readonly IMessagingHub _hub;

    public CalculateRequestHandler(IMessagingHub hub)
    {
        _hub = hub;
    }

    public override async Task<double> HandleAsync(CalculateRequest request, CancellationToken cancellation)
    {
        switch (request.Operator)
        {
            case CalculationOperator.Add:
                return await _hub.SendAsync(new AddRequest { Operand1 = request.Operand1, Operand2 = request.Operand2 });
            case CalculationOperator.Subtract:
                return await _hub.SendAsync(new SubRequest { Operand1 = request.Operand1, Operand2 = request.Operand2 });
            case CalculationOperator.Multiply:
                return await _hub.SendAsync(new MultiplyRequest { Operand1 = request.Operand1, Operand2 = request.Operand2 });
            case CalculationOperator.Divide:
                return await _hub.SendAsync(new DivideRequest { Operand1 = request.Operand1, Operand2 = request.Operand2 });
            default:
                throw new NotSupportedException($"Operator '{request.Operator}' is not supported.");
        }
    }
}