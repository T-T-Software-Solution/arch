using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Products;

public sealed record CreateProduct : IRequesting<ProductVm>
{
    public required string Name { get; init; }
    public required double Price { get; init; }
}

internal sealed class CreateProductHandler(IRepository<Product> repository, IMapper mapper) : RequestHandlerAsync<CreateProduct, ProductVm>
{
    public override async Task<ProductVm> HandleAsync(CreateProduct request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.Name) && request.Price > 0;
        if (false == areArgumentsValid)
        {
            return null;
        }

        var entity = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Price = request.Price
        };
        await repository.InsertAsync(entity);
        return mapper.Map<ProductVm>(entity);
    }
}