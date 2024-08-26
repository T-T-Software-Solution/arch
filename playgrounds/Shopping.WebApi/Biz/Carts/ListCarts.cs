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
        var paging = await repository
            .Include(it => it.Owner)
            .Include(it => it.Products)
            .ExcludeDeleted()
            .GetPaging(request.PageNo, request.PageSize)
            .ExecuteAsync<CartVm>();

        return Response(HttpStatusCode.OK, paging);
    }
}