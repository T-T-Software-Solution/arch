using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Products;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Controllers;

public sealed class ProductsController(IMessagingHub hub) : ApiControllerBase
{
    [Authorize]
    [HttpPost("create")]
    public Task<ProductVm> Create([FromBody] CreateProduct request)
       => hub.SendAsync(request);

    [HttpGet("{id}")]
    public Task<ProductVm> Get(string id)
        => hub.SendAsync(new GetProduct(id));

    [HttpGet("list")]
    public Task<ActionResult<IPagingResponse<ProductVm>>> Liste([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 30, [FromQuery] string? keyword = default)
        => hub.SendAsync(new ListProducts { PageNo = pageNo, PageSize = pageSize, Keyword = keyword }).ToActionResultAsync();

    [Authorize]
    [HttpPut("update/{id}")]
    public Task<ProductVm> Update(string id, [FromBody] UpdateProduct request)
        => hub.SendAsync(request with { ProductId = id });

    [Authorize]
    [HttpDelete("delete/{id}")]
    public Task Delete(string id)
        => hub.SendAsync(new DeleteProduct(id));
}