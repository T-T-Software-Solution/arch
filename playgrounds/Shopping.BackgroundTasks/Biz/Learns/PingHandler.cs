using Shopping.Shared.Requests.Learns;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.BackgroundTasks.Biz.Learns;

public sealed class PingHandler : RemoteRequestHandlerAsync<Ping, Pong>
{
    public override Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken = default)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[RootId: {RemoteContext.InitiatorId} | CurrentId: {RemoteContext.CorrelationId}] ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Received: {request}");
        Console.ForegroundColor = ConsoleColor.White;
        var response = new Pong(request.First + request.Second);
        return Task.FromResult(response);
    }
}