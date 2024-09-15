using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample03.Basic.ConsoleApp.ProjectStructure.Calculators;

public sealed record Multiply(double Operand1, double Operand2) : IRequesting<double>;

file sealed class Handler(IMessagingHub hub) : RequestHandlerAsync<Multiply, double>
{
    public override async Task<double> HandleAsync(Multiply request, CancellationToken cancellationToken = default)
    {
        var sum = 0d;
        for (var i = 0; i < request.Operand2; i++)
        {
            sum = await hub.SendAsync(new Add(request.Operand1, sum));
        }
        return sum;
    }
}