using TTSS.Core.Messaging;

namespace Shopping.Shared.Requests.Learns;

public sealed record Greeting : IRemoteRequesting
{
    public string Message { get; set; }
}