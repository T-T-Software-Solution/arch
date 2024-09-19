using Sample18.RemotePublish.Shared.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample18.RemotePublish.ConsoleApp2.Handlers;

public sealed class RegiserNewOperatorHandler : RemotePublicationHandlerAsync<NotifyUserRegistered>
{
    private readonly IEnumerable<UserRole> _targetRoles = [UserRole.Admin, UserRole.Moderator];

    public override Task HandleAsync(NotifyUserRegistered request, CancellationToken cancellationToken = default)
    {
        if (false == _targetRoles.Contains(request.Role))
        {
            Console.WriteLine($"[IGNORED] {request}");
            return Task.CompletedTask;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"New operator registered: {request.Username} with role {request.Role}");
        Console.ResetColor();
        return Task.CompletedTask;
    }
}