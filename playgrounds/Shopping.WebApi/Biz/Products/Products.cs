using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Web.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed class Products(IMessagingHub hub) : ApiControllerBase
{
    [Authorize]
    [HttpPost]
    public Task<ActionResult<ProductVm>> Create([FromBody] ProductsCreate request)
       => hub.SendAsync(request).ToActionResultAsync();

    [HttpGet("{id}")]
    public Task<ActionResult<ProductVm>> Get(string id)
        => hub.SendAsync(new ProductsGet(id)).ToActionResultAsync();

    [HttpGet]
    public Task<ActionResult<Paging<ProductVm>>> Liste([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 30, [FromQuery] string? keyword = default)
        => hub.SendAsync(new ProductsList { PageNo = pageNo, PageSize = pageSize, Keyword = keyword }).ToActionResultAsync();

    [Authorize]
    [HttpPut("{id}")]
    public Task<ActionResult<ProductVm>> Update(string id, [FromBody] ProductsUpdate request)
        => hub.SendAsync(request with { ProductId = id }).ToActionResultAsync();

    [Authorize]
    [HttpDelete("{id}")]
    public Task<ActionResult> Delete(string id)
        => hub.SendAsync(new ProductsDelete(id)).ToActionResultAsync();
}