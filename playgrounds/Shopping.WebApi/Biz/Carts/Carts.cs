using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Web.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Carts;

public sealed class Carts(IMessagingHub hub) : ApiControllerBase
{
    [Authorize]
    [HttpPost]
    public Task<ActionResult<CartVm>> Create()
     => hub.SendAsync(new CartsCreate()).ToActionResultAsync();

    [HttpGet("{id}")]
    public Task<ActionResult<CartVm>> Get(string id)
       => hub.SendAsync(new CartsGet(id)).ToActionResultAsync();

    [HttpGet]
    public Task<ActionResult<Paging<CartVm>>> List([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 30, [FromQuery] string? keyword = default)
        => hub.SendAsync(new CartsList { PageNo = pageNo, PageSize = pageSize, Keyword = keyword }).ToActionResultAsync();

    [Authorize]
    [HttpPut("{id}")]
    public Task<ActionResult<CartVm>> Update(string id, [FromBody] CartsUpdate request)
        => hub.SendAsync(request with { CartId = id }).ToActionResultAsync();

    [Authorize]
    [HttpDelete("{id}")]
    public Task<ActionResult> Delete(string id)
        => hub.SendAsync(new CartsDelete(id)).ToActionResultAsync();
}