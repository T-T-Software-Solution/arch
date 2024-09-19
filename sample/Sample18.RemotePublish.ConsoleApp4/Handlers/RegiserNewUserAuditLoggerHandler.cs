using Sample18.RemotePublish.Shared.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample18.RemotePublish.ConsoleApp4.Handlers;

public sealed class RegiserNewUserAuditLoggerHandler : RemotePublicationHandlerAsync<NotifyUserRegistered>
{
    public override Task HandleAsync(NotifyUserRegistered request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Add audit log about: {request}");
        return Task.CompletedTask;
    }
}