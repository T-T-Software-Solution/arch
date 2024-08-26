using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Carts;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Controllers;

public sealed class CartsController(IMessagingHub hub) : ApiControllerBase
{
    [Authorize]
    [HttpPost("create")]
    public Task<ActionResult<CartVm>> Create()
     => hub.SendAsync(new CreateCart()).ToActionResultAsync();

    [HttpGet("{id}")]
    public Task<ActionResult<CartVm>> Get(string id)
       => hub.SendAsync(new GetCart(id)).ToActionResultAsync();

    [HttpGet("list")]
    public Task<ActionResult<Paging<CartVm>>> List([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 30, [FromQuery] string? keyword = default)
        => hub.SendAsync(new ListCarts { PageNo = pageNo, PageSize = pageSize, Keyword = keyword }).ToActionResultAsync();

    [Authorize]
    [HttpPut("update/{id}")]
    public Task<ActionResult<CartVm>> Update(string id, [FromBody] UpdateCart request)
        => hub.SendAsync(request with { CartId = id }).ToActionResultAsync();

    [Authorize]
    [HttpDelete("delete/{id}")]
    public Task<ActionResult> Delete(string id)
        => hub.SendAsync(new DeleteCart(id)).ToActionResultAsync();
}