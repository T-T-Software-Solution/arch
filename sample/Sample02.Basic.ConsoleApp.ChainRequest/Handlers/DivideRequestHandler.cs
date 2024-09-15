using Sample02.Basic.ConsoleApp.ChainRequest.Messages;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample02.Basic.ConsoleApp.ChainRequest.Handlers;

internal sealed class DivideRequestHandler : RequestHandlerAsync<DivideRequest, double>
{
    private readonly IMessagingHub _hub;

    public DivideRequestHandler(IMessagingHub hub)
    {
        _hub = hub;
    }

    public override async Task<double> HandleAsync(DivideRequest request, CancellationToken cancellationToken = default)
    {
        var round = 0;
        var remainder = request.Operand1;
        while (remainder > request.Operand2)
        {
            round++;
            remainder = await _hub.SendAsync(new SubRequest { Operand1 = remainder, Operand2 = request.Operand2 });
        }
        return round;
    }
}