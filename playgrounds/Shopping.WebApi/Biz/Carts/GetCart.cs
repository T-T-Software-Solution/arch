using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Carts;

public sealed record GetCart(string CartId) : IRequesting<CartVm>;

internal class GetCartHandler(IRepository<Cart> repository, IMapper mapper) : RequestHandlerAsync<GetCart, CartVm>
{
    public override async Task<CartVm> HandleAsync(GetCart request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.CartId, cancellationToken);
        if (entity is null) return null;

        return mapper.Map<CartVm>(entity);
    }
}