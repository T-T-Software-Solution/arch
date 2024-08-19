using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Carts;

public sealed record CreateCart : IRequesting<CartVm>;

internal sealed class CreateCartHandler(ICorrelationContext context,
    IRepository<User> userRepository,
    IRepository<Cart> cartRepository,
    IMapper mapper) : RequestHandlerAsync<CreateCart, CartVm>
{
    public override async Task<CartVm> HandleAsync(CreateCart request, CancellationToken cancellationToken = default)
    {
        if (context.CurrentUserId is null)
        {
            return null;
        }

        var user = await userRepository.GetByIdAsync(context.CurrentUserId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var entity = new Cart
        {
            Id = Guid.NewGuid().ToString(),
            Owner = user,
        };
        await cartRepository.InsertAsync(entity);
        return mapper.Map<CartVm>(entity);
    }
}