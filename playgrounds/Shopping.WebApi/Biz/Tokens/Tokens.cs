﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TTSS.Core.Web.Controllers;
using TTSS.Core.Messaging;

namespace Shopping.WebApi.Biz.Tokens;

public sealed class Tokens(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost]
    public Task<string> Post([FromBody] CreateToken request)
        => hub.SendAsync(request);

    [Authorize]
    [HttpGet("CheckAuth")]
    public string? Get()
        => string.Join(", ", [
            $"ID:{HttpContext.User.Identity?.Name}",
            $"DisplayName:{HttpContext.User.Claims.FirstOrDefault(it=>it.Type == ClaimTypes.GivenName)?.Value}", ]);
}