using TTSS.Core.Messaging;

namespace Sample02.Basic.ConsoleApp.ChainRequest.Messages;

public sealed record DivideRequest : IRequesting<double>
{
    public double Operand1 { get; init; }
    public double Operand2 { get; init; }
}