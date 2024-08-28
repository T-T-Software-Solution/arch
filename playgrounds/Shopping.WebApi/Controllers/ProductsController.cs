using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Products;
using TTSS.Core.Web.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Controllers;

public sealed class ProductsController(IMessagingHub hub) : ApiControllerBase
{
    [Authorize]
    [HttpPost("create")]
    public Task<ActionResult<ProductVm>> Create([FromBody] CreateProduct request)
       => hub.SendAsync(request).ToActionResultAsync();

    [HttpGet("{id}")]
    public Task<ActionResult<ProductVm>> Get(string id)
        => hub.SendAsync(new GetProduct(id)).ToActionResultAsync();

    [HttpGet("list")]
    public Task<ActionResult<Paging<ProductVm>>> Liste([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 30, [FromQuery] string? keyword = default)
        => hub.SendAsync(new ListProducts { PageNo = pageNo, PageSize = pageSize, Keyword = keyword }).ToActionResultAsync();

    [Authorize]
    [HttpPut("update/{id}")]
    public Task<ActionResult<ProductVm>> Update(string id, [FromBody] UpdateProduct request)
        => hub.SendAsync(request with { ProductId = id }).ToActionResultAsync();

    [Authorize]
    [HttpDelete("delete/{id}")]
    public Task<ActionResult> Delete(string id)
        => hub.SendAsync(new DeleteProduct(id)).ToActionResultAsync();
}