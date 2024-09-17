using TTSS.Core.Messaging;

namespace Sample11.Basic.WebApp.SetupProject.Messages;

public sealed record Ping : IHttpRequesting<Pong>
{
    public required string Name { get; init; }
}

public sealed record Pong
{
    public required string Response { get; init; }
}