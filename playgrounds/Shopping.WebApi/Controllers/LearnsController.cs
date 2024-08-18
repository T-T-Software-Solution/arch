using Microsoft.AspNetCore.Mvc;
using Shopping.WebApi.Biz.Learns;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Controllers;

public sealed class LearnsController(IMessagingHub hub) : ApiControllerBase
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
}
