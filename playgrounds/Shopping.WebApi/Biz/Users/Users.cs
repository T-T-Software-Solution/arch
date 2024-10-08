﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Users.ViewModels;
using TTSS.Core.Messaging;
using TTSS.Core.Models;
using TTSS.Core.Web.Controllers;

namespace Shopping.WebApi.Biz.Users;

public sealed class Users(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost]
    public Task<ActionResult<CreateUserResult>> Create([FromBody] UsersCreate request)
        => hub.SendAsync(request).ToActionResultAsync();

    [HttpGet("{id}")]
    public Task<ActionResult<UserVm>> Get(string id)
        => hub.SendAsync(new UsersGet(id)).ToActionResultAsync();

    [HttpGet]
    public Task<ActionResult<Paging<UserVm>>> Liste([FromQuery] int pageNo = 1,
        [FromQuery] int pageSize = 30,
        [FromQuery] IEnumerable<string>? sort = default,
        [FromQuery] Dictionary<string, string>? filter = default)
        => hub.SendAsync(new UsersList { PageNo = pageNo, PageSize = pageSize, Sort = sort, Filter = filter }).ToActionResultAsync();

    [Authorize]
    [HttpPut("{id}")]
    public Task<ActionResult<UserVm>> Update(string id, [FromBody] UsersUpdate request)
        => hub.SendAsync(request with { UserId = id }).ToActionResultAsync();

    [Authorize]
    [HttpDelete("{id}")]
    public Task<ActionResult> Delete(string id)
        => hub.SendAsync(new UsersDelete(id)).ToActionResultAsync();
}