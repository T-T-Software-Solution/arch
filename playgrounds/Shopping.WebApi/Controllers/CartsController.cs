using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Carts;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;

namespace Shopping.WebApi.Controllers;

public sealed class CartsController(IMessagingHub hub) : ApiControllerBase
{
    [Authorize]
    [HttpPost("create")]
    public Task<CartVm> Create()
     => hub.SendAsync(new CreateCart());

    [HttpGet("{id}")]
    public Task<CartVm> Get(string id)
       => hub.SendAsync(new GetCart(id));

    [HttpGet("list")]
    public Task<IEnumerable<CartVm>> List()
        => hub.SendAsync(new ListCarts());

    [Authorize]
    [HttpPut("update")]
    public Task<CartVm> Update([FromBody] UpdateCart request)
        => hub.SendAsync(request);

    [Authorize]
    [HttpDelete("delete")]
    public Task Delete([FromBody] DeleteCart request)
        => hub.SendAsync(request);
}