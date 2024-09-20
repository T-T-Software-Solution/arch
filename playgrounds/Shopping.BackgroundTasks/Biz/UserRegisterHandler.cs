using Shopping.Shared.Requests;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.BackgroundTasks.Biz;

public sealed class UserRegisterHandler : RemotePublicationHandlerAsync<UserRegistered>
{
    public override Task HandleAsync(UserRegistered request, CancellationToken cancellationToken = default)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[RootId: {RemoteContext.InitiatorId} | CurrentId: {RemoteContext.CorrelationId}] ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(request.ToString());
        Console.ResetColor();
        return Task.CompletedTask;
    }
}