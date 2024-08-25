using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using System.Text.Json.Serialization;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed record UpdateProduct : IHttpRequesting<ProductVm>
{
    [JsonIgnore]
    public string? ProductId { get; init; }
    public string? Name { get; init; }
    public double? Price { get; init; }
}

internal sealed class UpdateProductHandler(IRepository<Product> repository, IMapper mapper)
    : HttpRequestHandlerAsync<UpdateProduct, ProductVm>
{
    public override async Task<IHttpResponse<ProductVm>> HandleAsync(UpdateProduct request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.ProductId)
            && (!string.IsNullOrWhiteSpace(request.Name) || request.Price is not null);
        if (!areArgumentsValid)
        {
            return Response(HttpStatusCode.BadRequest, "Invalid arguments");
        }

        var entity = await repository.GetByIdAsync(request.ProductId!, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.NotFound, "Product not found");
        }

        if (request.Name is not null)
        {
            entity.Name = request.Name;
        }

        if (request.Price is not null)
        {
            entity.Price = request.Price.Value;
        }

        var ack = await repository.UpdateAsync(entity, cancellationToken);
        if (ack is false)
        {
            return Response(HttpStatusCode.InternalServerError, "Failed to update product");
        }

        var vm = mapper.Map<ProductVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}