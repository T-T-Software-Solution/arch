using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Messaging;
using TTSS.Core.Models;
using TTSS.Core.Web.Controllers;

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
    public Task<ActionResult<Paging<CartVm>>> List([FromQuery] int pageNo = 1,
        [FromQuery] int pageSize = 30,
        [FromQuery] IEnumerable<string>? sort = default,
        [FromQuery] Dictionary<string, string>? filter = default)
        => hub.SendAsync(new CartsList { PageNo = pageNo, PageSize = pageSize, Sort = sort, Filter = filter }).ToActionResultAsync();

    [Authorize]
    [HttpPut("{id}")]
    public Task<ActionResult<CartVm>> Update(string id, [FromBody] CartsUpdate request)
        => hub.SendAsync(request with { CartId = id }).ToActionResultAsync();

    [Authorize]
    [HttpDelete("{id}")]
    public Task<ActionResult> Delete(string id)
        => hub.SendAsync(new CartsDelete(id)).ToActionResultAsync();
}