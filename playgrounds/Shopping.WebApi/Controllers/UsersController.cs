using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Users;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;

namespace Shopping.WebApi.Controllers;

public sealed class UsersController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost("create")]
    public Task<string> Create([FromBody] CreateUser request)
        => hub.SendAsync(request);

    [HttpGet("{id}")]
    public Task<UserVm> Get(string id)
        => hub.SendAsync(new GetUser(id));

    [HttpGet("list")]
    public Task<IEnumerable<UserVm>> Liste()
        => hub.SendAsync(new ListUsers());

    [Authorize]
    [HttpPut("update")]
    public Task<UserVm> Update([FromBody] UpdateUser request)
        => hub.SendAsync(request);

    [Authorize]
    [HttpDelete("delete")]
    public Task Delete([FromBody] DeleteUser request)
        => hub.SendAsync(request);
}