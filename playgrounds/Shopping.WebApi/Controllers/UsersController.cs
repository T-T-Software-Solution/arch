using Microsoft.AspNetCore.Authorization;
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
    public Task<CreateUserResult> Create([FromBody] CreateUser request)
        => hub.SendAsync(request);

    [HttpGet("{id}")]
    public Task<UserVm> Get(string id)
        => hub.SendAsync(new GetUser(id));

    [HttpGet("list")]
    public Task<IEnumerable<UserVm>> Liste()
        => hub.SendAsync(new ListUsers());

    [Authorize]
    [HttpPut("update/{id}")]
    public Task<UserVm> Update(string id, [FromBody] UpdateUser request)
        => hub.SendAsync(request with { UserId = id });

    [Authorize]
    [HttpDelete("delete/{id}")]
    public Task Delete(string id)
        => hub.SendAsync(new DeleteUser(id));
}