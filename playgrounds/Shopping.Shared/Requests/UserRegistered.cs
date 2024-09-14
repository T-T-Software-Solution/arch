using TTSS.Core.Messaging;

namespace Shopping.Shared.Requests;

public sealed record UserRegistered : IRemoteRequesting
{
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}