using Sample11.Basic.WebApp.SetupProject.Messages;
using System.Net;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample11.Basic.WebApp.SetupProject.Handlers;

file sealed class PingHandler(ILogger<Ping> logger) : HttpRequestHandler<Ping, Pong>
{
    public override IHttpResponse<Pong> Handle(Ping request)
    {
        logger.LogInformation($"Received: {request.Name}");
        var result = new Pong
        {
            Response = $"Pong, {request.Name}!"
        };
        return Response(HttpStatusCode.OK, result);
    }
}