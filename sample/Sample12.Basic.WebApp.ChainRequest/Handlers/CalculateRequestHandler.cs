using Sample12.Basic.WebApp.ChainRequest.Messages;
using System.Net;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample12.Basic.WebApp.ChainRequest.Handlers;

internal sealed class CalculateRequestHandler : HttpRequestHandlerAsync<CalculateRequest, double>
{
    private readonly IMessagingHub _hub;

    public CalculateRequestHandler(IMessagingHub hub)
    {
        _hub = hub;
    }

    public override async Task<IHttpResponse<double>> HandleAsync(CalculateRequest request, CancellationToken cancellation)
    {
        var result = 0d;
        switch (request.Operator)
        {
            case CalculationOperator.Add:
                result = await _hub.SendAsync(new AddRequest { Operand1 = request.Operand1, Operand2 = request.Operand2 });
                break;
            case CalculationOperator.Subtract:
                result = await _hub.SendAsync(new SubRequest { Operand1 = request.Operand1, Operand2 = request.Operand2 });
                break;
            case CalculationOperator.Multiply:
                result = await _hub.SendAsync(new MultiplyRequest { Operand1 = request.Operand1, Operand2 = request.Operand2 });
                break;
            case CalculationOperator.Divide:
                result = await _hub.SendAsync(new DivideRequest { Operand1 = request.Operand1, Operand2 = request.Operand2 });
                break;
            default:
                return Response(HttpStatusCode.BadRequest, $"Operator '{request.Operator}' is not supported.");
        }
        return Response(HttpStatusCode.OK, result);
    }
}