using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample03.Basic.ConsoleApp.ProjectStructure.Calculators;

public sealed record Sub(double Operand1, double Operand2) : IRequesting<double>;

file sealed class Handler : RequestHandler<Sub, double>
{
    public override double Handle(Sub request)
        => request.Operand1 - request.Operand2;
}