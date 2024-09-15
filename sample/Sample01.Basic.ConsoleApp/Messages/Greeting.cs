using TTSS.Core.Messaging;

namespace Sample01.Basic.ConsoleApp.Messages;

public sealed record Greeting : IRequesting
{
    public required string Message { get; init; }
}