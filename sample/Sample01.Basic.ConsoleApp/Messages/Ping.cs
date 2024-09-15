using TTSS.Core.Messaging;

namespace Sample01.Basic.ConsoleApp.Messages;

public sealed record Ping : IRequesting<Pong>
{
    public required string Name { get; init; }
}

public sealed record Pong
{
    public required string Response { get; init; }
}