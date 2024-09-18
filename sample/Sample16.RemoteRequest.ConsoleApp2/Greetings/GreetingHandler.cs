using Sample16.RemoteRequest.Shared.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample16.RemoteRequest.ConsoleApp2.Greetings;

public sealed class GreetingHandler : RemoteRequestHandlerAsync<Greeting>
{
    public override Task HandleAsync(Greeting request, CancellationToken cancellationToken = default)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Receive: {request}");
        Console.ForegroundColor = ConsoleColor.White;
        return Task.CompletedTask;
    }
}