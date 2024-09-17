using TTSS.Core.Messaging;

namespace Sample11.Basic.WebApp.SetupProject.Messages;

public sealed record Greeting : IHttpRequesting
{
    public required string Message { get; init; }
}