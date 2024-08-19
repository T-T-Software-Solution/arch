using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.WebApi.Biz.Tokens;
using System.Security.Claims;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Controllers;

public sealed class TokensController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost]
    public Task<Response> Post([FromBody] CreateToken request)
        => hub.SendAsync(request);

    [Authorize]
    [HttpGet("CheckAuth")]
    public string? Get()
        => string.Join(", ", [
            $"ID:{HttpContext.User.Identity?.Name}",
            $"DisplayName:{HttpContext.User.Claims.FirstOrDefault(it=>it.Type == ClaimTypes.GivenName)?.Value}", ]);
}