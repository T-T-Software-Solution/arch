using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Users;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Controllers;

public sealed class UsersController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost("create")]
    public Task<Response> Create([FromBody] CreateUser request)
        => hub.SendAsync(request);

    [HttpGet("{id}")]
    public Task<UserVm> Get(string id)
        => hub.SendAsync(new GetUser(id));

    [HttpGet("list")]
    public Task<IEnumerable<UserVm>> Liste()
        => hub.SendAsync(new ListUsers());

    [Authorize]
    [HttpPut("update/{id}")]
    public Task<UserVm> Update(string id)
        => hub.SendAsync(new UpdateUser(id));

    [Authorize]
    [HttpDelete("delete/{id}")]
    public Task Delete(string id)
        => hub.SendAsync(new DeleteUser(id));
}