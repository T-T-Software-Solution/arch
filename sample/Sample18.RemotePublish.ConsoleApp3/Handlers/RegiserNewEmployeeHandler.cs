using Sample18.RemotePublish.Shared.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample18.RemotePublish.ConsoleApp3.Handlers;

public sealed class RegiserNewEmployeeHandler : RemotePublicationHandlerAsync<NotifyUserRegistered>
{
    private readonly IEnumerable<UserRole> _targetRoles = [UserRole.Employee];

    public override Task HandleAsync(NotifyUserRegistered request, CancellationToken cancellationToken = default)
    {
        if (false == _targetRoles.Contains(request.Role))
        {
            Console.WriteLine($"[IGNORED] {request}");
            return Task.CompletedTask;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"New employee registered: {request.Username}");
        Console.ResetColor();
        return Task.CompletedTask;
    }
}