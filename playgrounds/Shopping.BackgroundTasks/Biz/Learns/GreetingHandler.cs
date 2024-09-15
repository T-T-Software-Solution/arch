using Shopping.Shared.Requests.Learns;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.BackgroundTasks.Biz.Learns;

public sealed class GreetingHandler : RemoteRequestHandlerAsync<Greeting>
{
    public override Task HandleAsync(Greeting request, CancellationToken cancellationToken = default)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[RootId: {RemoteContext.InitiatorId} | CurrentId: {RemoteContext.CorrelationId}] ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Received: {request.Message}");
        Console.ForegroundColor = ConsoleColor.White;
        return Task.CompletedTask;
    }
}