using Microsoft.Extensions.Logging;
using Shopping.Shared.Requests.Learns;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.BackgroundTasks.Biz.Learns;

public sealed class GreetingHandler(ILogger<Greeting> logger) : RemoteRequestHandlerAsync<Greeting>
{
    public override Task HandleAsync(Greeting request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Received message: {@message}", request.Message);
        return Task.CompletedTask;
    }
}