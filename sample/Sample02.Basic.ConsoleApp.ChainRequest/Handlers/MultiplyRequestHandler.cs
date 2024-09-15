using Sample02.Basic.ConsoleApp.ChainRequest.Messages;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample02.Basic.ConsoleApp.ChainRequest.Handlers;

internal sealed class MultiplyRequestHandler : RequestHandlerAsync<MultiplyRequest, double>
{
    private readonly IMessagingHub _hub;

    public MultiplyRequestHandler(IMessagingHub hub)
    {
        _hub = hub;
    }

    public override async Task<double> HandleAsync(MultiplyRequest request, CancellationToken cancellationToken = default)
    {
        var sum = 0d;
        for (var i = 0; i < request.Operand2; i++)
        {
            sum = await _hub.SendAsync(new AddRequest { Operand1 = request.Operand1, Operand2 = sum });
        }
        return sum;
    }
}