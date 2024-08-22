﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Users;
using Shopping.WebApi.Biz.Users.ViewModels;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;

namespace Shopping.WebApi.Controllers;

public sealed class UsersController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost("create")]
    public Task<ActionResult<CreateUserResult>> Create([FromBody] CreateUser request)
        => hub.SendAsync(request).ToActionResultAsync();

    [HttpGet("{id}")]
    public Task<ActionResult<UserVm>> Get(string id)
        => hub.SendAsync(new GetUser(id)).ToActionResultAsync();

    [HttpGet("list")]
    public Task<ActionResult<IEnumerable<UserVm>>> Liste()
        => hub.SendAsync(new ListUsers()).ToActionResultAsync();

    [Authorize]
    [HttpPut("update/{id}")]
    public Task<ActionResult<UserVm>> Update(string id, [FromBody] UpdateUser request)
        => hub.SendAsync(request with { UserId = id }).ToActionResultAsync();

    [Authorize]
    [HttpDelete("delete/{id}")]
    public Task<ActionResult> Delete(string id)
        => hub.SendAsync(new DeleteUser(id)).ToActionResultAsync();
}