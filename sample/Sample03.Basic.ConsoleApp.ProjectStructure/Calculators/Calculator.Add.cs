using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample03.Basic.ConsoleApp.ProjectStructure.Calculators;

public sealed record Add(double Operand1, double Operand2) : IRequesting<double>;

file sealed class Handler : RequestHandler<Add, double>
{
    public override double Handle(Add request)
        => request.Operand1 + request.Operand2;
}