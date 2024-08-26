using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed record ListProducts : IHttpRequesting<Paging<ProductVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public string? Keyword { get; init; }
}

internal sealed class ListProductHandler(IRepository<Product> repository)
    : HttpRequestHandlerAsync<ListProducts, Paging<ProductVm>>
{
    public override async Task<IHttpResponse<Paging<ProductVm>>> HandleAsync(ListProducts request, CancellationToken cancellationToken = default)
    {
        var paging = await repository
            .ExcludeDeleted()
            .GetPaging(request.PageNo, request.PageSize)
            .ExecuteAsync<ProductVm>();
        return Response(System.Net.HttpStatusCode.OK, paging);
    }
}