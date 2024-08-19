using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Text.Json.Serialization;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Products;

public sealed record UpdateProduct : IRequesting<ProductVm>
{
    [JsonIgnore]
    public string? ProductId { get; init; }
    public string? Name { get; init; }
    public double? Price { get; init; }
}

internal sealed class UpdateProductHandler(IRepository<Product> repository, IMapper mapper) : RequestHandlerAsync<UpdateProduct, ProductVm>
{
    public override async Task<ProductVm> HandleAsync(UpdateProduct request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.ProductId);
        if (!areArgumentsValid)
        {
            return null;
        }

        var entity = await repository.GetByIdAsync(request.ProductId, cancellationToken);
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

        await repository.UpdateAsync(entity, cancellationToken);
        return mapper.Map<ProductVm>(entity);
    }
}