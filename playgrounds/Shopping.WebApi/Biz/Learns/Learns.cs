using Microsoft.AspNetCore.Mvc;
using TTSS.Core.Messaging;
using TTSS.Core.Models;
using TTSS.Core.Web.Controllers;

namespace Shopping.WebApi.Biz.Learns;

public sealed class Learns(IMessagingHub hub, ICorrelationContext context) : ApiControllerBase
{
    [HttpGet("1/OneWay/{input}")]
    public Task OneWay(string input)
        => hub.SendAsync(new OneWay { Input = input });

    [HttpPost("2/TwoWay")]
    public Task<Response> TwoWay([FromBody] TwoWay request)
        => hub.SendAsync(request);

    [HttpPut("3/ChainCalls")]
    public Task<Response> ChainCall([FromBody] ChainCalls request)
        => hub.SendAsync(request);

    [HttpGet("4/Correlation")]
    public ICorrelationContext GetCollelationContext()
        => context;
}
