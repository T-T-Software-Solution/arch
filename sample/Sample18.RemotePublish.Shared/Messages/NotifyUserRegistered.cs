using TTSS.Core.Messaging;

namespace Sample18.RemotePublish.Shared.Messages;

public sealed record NotifyUserRegistered : IRemotePublication
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public UserRole Role { get; set; }
}