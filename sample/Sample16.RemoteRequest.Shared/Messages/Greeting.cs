using TTSS.Core.Messaging;

namespace Sample16.RemoteRequest.Shared.Messages;

public sealed record Greeting(string Message) : IRemoteRequesting;