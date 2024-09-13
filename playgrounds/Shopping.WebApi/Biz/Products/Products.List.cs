﻿using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed record ProductsList : IHttpRequesting<Paging<ProductVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public string? Keyword { get; init; }
}

file sealed class Handler(IRepository<Product> repository)
    : HttpRequestHandlerAsync<ProductsList, Paging<ProductVm>>
{
    public override async Task<IHttpResponse<Paging<ProductVm>>> HandleAsync(ProductsList request, CancellationToken cancellationToken = default)
    {
        var keyword = request.Keyword?.ToLower() ?? string.Empty;
        var shouldSkipSearch = string.IsNullOrEmpty(keyword);
        var paging = await repository
            .ExcludeDeleted()
            .GetPaging(request.PageNo, request.PageSize, it => shouldSkipSearch
            || (null != it.Id && it.Id.Contains(keyword))
            || (null != it.Name && it.Name.Contains(keyword))
            || it.Price.ToString().Contains(keyword))
            .ExecuteAsync<ProductVm>();
        return Response(System.Net.HttpStatusCode.OK, paging);
    }
}