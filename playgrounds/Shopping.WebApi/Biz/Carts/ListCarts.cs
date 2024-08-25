using Microsoft.EntityFrameworkCore;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Carts;

public sealed record ListCarts : IHttpRequesting<Paging<CartVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public string? Keyword { get; init; }
}

internal sealed class ListCartsHandler(IRepository<Cart> repository)
    : HttpRequestHandlerAsync<ListCarts, Paging<CartVm>>
{
    public override async Task<IHttpResponse<Paging<CartVm>>> HandleAsync(ListCarts request, CancellationToken cancellationToken = default)
    {
        // TODO: Simplify this later
        if (repository is IConfigurableRepository<Cart> confiure)
        {
            confiure.Configure(table => table
                .Include(cart => cart.Owner)
                .Include(cart => cart.Products));
        }
        var paging = await repository
            .ExcludeDeleted()
            .GetPaging(request.PageNo, request.PageSize)
            .ExecuteAsync<CartVm>();

        return Response(HttpStatusCode.OK, paging);
    }
}