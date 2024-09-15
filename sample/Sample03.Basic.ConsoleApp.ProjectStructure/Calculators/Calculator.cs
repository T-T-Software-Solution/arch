using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Sample03.Basic.ConsoleApp.ProjectStructure.Calculators;

public sealed record class Calculator : IRequesting<double>
{
    public double Operand1 { get; init; }
    public double Operand2 { get; init; }
    public CalculationOperator Operator { get; init; }
}

file sealed class Handler(IMessagingHub hub) : RequestHandlerAsync<Calculator, double>
{
    public override async Task<double> HandleAsync(Calculator request, CancellationToken cancellation)
    {
        IRequesting<double> message;
        switch (request.Operator)
        {
            case CalculationOperator.Add:
                message = new Add(request.Operand1, request.Operand2);
                break;
            case CalculationOperator.Subtract:
                message = new Sub(request.Operand1, request.Operand2);
                break;
            case CalculationOperator.Multiply:
                message = new Multiply(request.Operand1, request.Operand2);
                break;
            case CalculationOperator.Divide:
                message = new Divide(request.Operand1, request.Operand2);
                break;
            default:
                throw new NotSupportedException($"Operator '{request.Operator}' is not supported.");
        }
        return await hub.SendAsync(message, cancellation);
    }
}