using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
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
    [HttpPut("update/{id}")]
    public Task<CartVm> Update(string id)
        => hub.SendAsync(new UpdateCart(id));

    [Authorize]
    [HttpDelete("delete/{id}")]
    public Task Delete(string id)
        => hub.SendAsync(new DeleteCart(id));
}