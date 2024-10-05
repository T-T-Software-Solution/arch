using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Facades;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed record ProductsList : IHttpRequesting<Paging<ProductVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public IEnumerable<string>? Sort { get; init; }
    public Dictionary<string, string>? Filter { get; init; }
}

file sealed class Handler(IRepository<Product> repository)
    : HttpRequestHandlerAsync<ProductsList, Paging<ProductVm>>
{
    public override async Task<IHttpResponse<Paging<ProductVm>>> HandleAsync(ProductsList request, CancellationToken cancellationToken = default)
    {
        var (order, filter) = request.ToExpressions<Product>();

        var paging = await repository
            .ExcludeDeleted()
            .GetPaging(request.PageNo, request.PageSize, filter, order)
            .ExecuteAsync<ProductVm>();
        return Response(System.Net.HttpStatusCode.OK, paging);
    }
}