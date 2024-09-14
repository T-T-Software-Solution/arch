using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Requests.Learns;
using TTSS.Core.Messaging;
using TTSS.Core.Models;
using TTSS.Core.Web.Controllers;

namespace Shopping.WebApi.Biz.Learns;

public sealed class Learns(IMessagingHub hub, IRemoteMessagingHub remoteHub, ICorrelationContext context) : ApiControllerBase
{
    [HttpGet("/Local/1/OneWay/{input}")]
    public Task OneWay(string input)
        => hub.SendAsync(new OneWay { Input = input });

    [HttpPost("/Local/2/TwoWay")]
    public Task<Response> TwoWay([FromBody] TwoWay request)
        => hub.SendAsync(request);

    [HttpPut("/Local/3/ChainCalls")]
    public Task<Response> ChainCall([FromBody] ChainCalls request)
        => hub.SendAsync(request);

    [HttpGet("/Local/4/Correlation")]
    public Task<ICorrelationContext> Correlation()
        => hub.SendAsync(new GetCorrelationContext());

    [HttpPost("/Remote/1/Greeting/{message}")]
    public Task SendGreeting(string message)
        => remoteHub.SendAsync(new Greeting { Message = message });

    [HttpPost("/Remote/2/Ping")]
    public Task<Pong?> SendGreeting([FromQuery] int first, [FromQuery] int second)
        => remoteHub.SendAsync<Ping, Pong>(new Ping(first, second));
}
