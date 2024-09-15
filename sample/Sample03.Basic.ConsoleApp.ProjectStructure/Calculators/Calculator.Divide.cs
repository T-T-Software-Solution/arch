using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample03.Basic.ConsoleApp.ProjectStructure.Calculators;

public sealed record Divide(double Operand1, double Operand2) : IRequesting<double>;

file sealed class Handler(IMessagingHub hub) : RequestHandlerAsync<Divide, double>
{
    public override async Task<double> HandleAsync(Divide request, CancellationToken cancellationToken = default)
    {
        var round = 0;
        var remainder = request.Operand1;
        while (remainder > request.Operand2)
        {
            round++;
            remainder = await hub.SendAsync(new Sub(remainder, request.Operand2));
        }
        return round;
    }
}