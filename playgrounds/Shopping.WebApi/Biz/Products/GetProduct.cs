using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Products;

public sealed record GetProduct(string ProductId) : IRequesting<ProductVm>;

internal sealed class GetProductHandler(IRepository<Product> repository, IMapper mapper) : RequestHandlerAsync<GetProduct, ProductVm>
{
    public override async Task<ProductVm> HandleAsync(GetProduct request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.ProductId);
        return mapper.Map<ProductVm>(entity);
    }
}