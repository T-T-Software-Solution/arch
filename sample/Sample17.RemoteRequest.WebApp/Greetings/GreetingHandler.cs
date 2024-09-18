using Sample16.RemoteRequest.Shared.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample17.RemoteRequest.WebApp.Greetings;

public sealed class GreetingHandler(ILogger<GreetingHandler> logger) : RemoteRequestHandlerAsync<Greeting>
{
    public override Task HandleAsync(Greeting request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Receive: {@request}", request);
        return Task.CompletedTask;
    }
}