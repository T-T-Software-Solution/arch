using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed record GetProduct(string ProductId) : IHttpRequesting<ProductVm>;

internal sealed class GetProductHandler(IRepository<Product> repository, IMapper mapper)
    : HttpRequestHandlerAsync<GetProduct, ProductVm>
{
    public override async Task<IHttpResponse<ProductVm>> HandleAsync(GetProduct request, CancellationToken cancellationToken = default)
    {
        var entity = await repository
            .ExcludeDeleted()
            .GetByIdAsync(request.ProductId, cancellationToken);

        if (entity is null)
        {
            return Response(HttpStatusCode.NotFound, "User not found");
        }

        var vm = mapper.Map<ProductVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}