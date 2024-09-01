using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed record ProductsGet(string ProductId) : IHttpRequesting<ProductVm>;

file sealed class Handler(IRepository<Product> repository, IMapper mapper)
    : HttpRequestHandlerAsync<ProductsGet, ProductVm>
{
    public override async Task<IHttpResponse<ProductVm>> HandleAsync(ProductsGet request, CancellationToken cancellationToken = default)
    {
        var entity = await repository
            .ExcludeDeleted()
            .GetByIdAsync(request.ProductId, cancellationToken);

        if (entity is null)
        {
            return Response(HttpStatusCode.Gone, "Product not found");
        }

        var vm = mapper.Map<ProductVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}