using AutoMapper;
using Shipping.Shared.Entities;
using Shipping.Shared.Entities.ViewModels;
using System.ComponentModel.DataAnnotations;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Carts;

public sealed record GetCart([Required] string CartId) : IRequesting<CartVm>;

internal class GetCartHandler(IRepository<Cart> repository, IMapper mapper) : RequestHandlerAsync<GetCart, CartVm>
{
    public override async Task<CartVm> HandleAsync(GetCart request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.CartId);
        if (entity is null) return null;

        return mapper.Map<CartVm>(entity);
    }
}