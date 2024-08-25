using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Products;

public sealed record CreateProduct : IHttpRequesting<ProductVm>
{
    public required string Name { get; init; }
    public required double Price { get; init; }
}

internal sealed class CreateProductHandler(IRepository<Product> repository, IMapper mapper)
    : HttpRequestHandlerAsync<CreateProduct, ProductVm>
{
    public override async Task<IHttpResponse<ProductVm>> HandleAsync(CreateProduct request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.Name) && request.Price > 0;
        if (false == areArgumentsValid)
        {
            return Response(HttpStatusCode.BadRequest, "Invalid arguments");
        }

        var entity = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Price = request.Price
        };
        await repository.InsertAsync(entity, cancellationToken);
        var vm = mapper.Map<ProductVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}