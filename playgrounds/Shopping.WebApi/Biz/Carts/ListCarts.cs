using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Carts;

public sealed record ListCarts : IRequesting<IEnumerable<CartVm>>;

internal sealed class ListCartsHandler(IRepository<Cart> repository, IMapper mapper) : RequestHandlerAsync<ListCarts, IEnumerable<CartVm>>
{
    public override async Task<IEnumerable<CartVm>> HandleAsync(ListCarts request, CancellationToken cancellationToken = default)
    {
        var entities = await repository.Query(cancellationToken)
            .Include(it => it.Owner)
            .Include(it => it.Products)
            .GetAsync();

        return entities.Select(mapper.Map<CartVm>);
    }
}
