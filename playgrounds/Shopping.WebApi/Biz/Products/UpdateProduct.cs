using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Products;

public sealed record UpdateProduct(string ProductId) : IRequesting<ProductVm>
{
    public string? Name { get; init; }
    public double? Price { get; init; }
}

internal sealed class UpdateProductHandler(IRepository<Product> repository, IMapper mapper) : RequestHandlerAsync<UpdateProduct, ProductVm>
{
    public override async Task<ProductVm> HandleAsync(UpdateProduct request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.ProductId);
        if (entity is null)
        {
            return null;
        }

        if (request.Name is not null)
        {
            entity.Name = request.Name;
        }

        if (request.Price is not null)
        {
            entity.Price = request.Price.Value;
        }

        await repository.UpdateAsync(entity);
        return mapper.Map<ProductVm>(entity);
    }
}