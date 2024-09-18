using TTSS.Core.Messaging;

namespace Sample16.RemoteRequest.Shared.Messages;

public sealed record Ping : IRemoteRequesting<Pong>
{
    public int First { get; init; }
    public int Second { get; init; }
}

public sealed record Pong
{
    public required string Message { get; init; }
    public int Result { get; init; }
}