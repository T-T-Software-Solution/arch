using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Carts;

public sealed record GetCart(string CartId) : IHttpRequesting<CartVm>;

internal class GetCartHandler(IRepository<Cart> repository, IMapper mapper)
    : HttpRequestHandlerAsync<GetCart, CartVm>
{
    public override async Task<IHttpResponse<CartVm>> HandleAsync(GetCart request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.CartId);
        if (!areArgumentsValid)
        {
            return Response(HttpStatusCode.BadRequest, "Invalid arguments");
        }

        // TODO: Simplify this later
        if (repository is IConfigurableRepository<Cart> confiure)
        {
            confiure.Configure(table => table
                .Include(cart => cart.Owner)
                .Include(cart => cart.Products));
        }

        var entity = await repository.GetByIdAsync(request.CartId, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.Gone, "Cart not found");
        }

        var vm = mapper.Map<CartVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}