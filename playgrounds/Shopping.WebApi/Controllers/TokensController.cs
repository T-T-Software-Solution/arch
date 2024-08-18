using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.WebApi.Biz.Tokens;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;

namespace Shopping.WebApi.Controllers;

public sealed class TokensController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost]
    public Task<string> Post([FromBody] CreateToken request)
        => hub.SendAsync(request);

    [Authorize]
    [HttpGet("CheckAuth")]
    public string? Get()
        => HttpContext.User.Identity?.Name;
}