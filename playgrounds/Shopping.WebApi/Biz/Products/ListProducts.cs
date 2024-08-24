using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed record ListProducts : IHttpRequesting<IPagingResponse<ProductVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public string? Keyword { get; init; }
}

internal sealed class ListProductHandler(IRepository<Product> repository)
    : HttpRequestHandlerAsync<ListProducts, IPagingResponse<ProductVm>>
{
    public override async Task<IHttpResponse<IPagingResponse<ProductVm>>> HandleAsync(ListProducts request, CancellationToken cancellationToken = default)
    {
        var paging = repository
            .ExcludeDelete()
            .GetPaging(request.PageNo, request.PageSize);
        var vm = await paging.ToPagingDataAsync<ProductVm>();
        return Response(System.Net.HttpStatusCode.OK, vm);
    }
}