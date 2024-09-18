using Sample16.RemoteRequest.Shared.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample16.RemoteRequest.ConsoleApp1.Pings;

public sealed class PingHandler : RemoteRequestHandlerAsync<Ping, Pong>
{
    public override Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken = default)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Receive: {request}");
        Console.ForegroundColor = ConsoleColor.White;

        var summation = request.First + request.Second;
        var response = new Pong
        {
            Result = summation,
            Message = $"(App1) The sum of {request.First} and {request.Second} is {summation}",
        };
        return Task.FromResult(response);
    }
}