using TTSS.Core.Messaging;

namespace Shopping.Shared.Requests.Learns;

public sealed record Ping(int First, int Second) : IRemoteRequesting<Pong>;
public sealed record Pong(int Result);
