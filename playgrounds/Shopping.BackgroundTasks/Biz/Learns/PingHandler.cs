using Microsoft.Extensions.Logging;
using Shopping.Shared.Requests.Learns;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.BackgroundTasks.Biz.Learns;

public sealed class PingHandler(ILogger<PingHandler> logger) : RemoteRequestHandlerAsync<Ping, Pong>
{
    public override Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Received message: {@First} and {@Second}", request.First, request.Second);
        var response = new Pong(request.First + request.Second);
        return Task.FromResult(response);
    }
}