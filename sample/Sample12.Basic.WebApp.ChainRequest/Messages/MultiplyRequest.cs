using TTSS.Core.Messaging;

namespace Sample12.Basic.WebApp.ChainRequest.Messages;

public sealed record MultiplyRequest : IRequesting<double>
{
    public double Operand1 { get; init; }
    public double Operand2 { get; init; }
}