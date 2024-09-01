using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using System.Text.Json.Serialization;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Carts;

public sealed record CartsUpdate : IHttpRequesting<CartVm>
{
    [JsonIgnore]
    public string? CartId { get; init; }
    public string? AddProductId { get; init; }
    public string? RemoveProductId { get; init; }
}

file sealed class Handler(ICorrelationContext context,
    IRepository<Cart> cartRepository,
    IRepository<Product> productRepository,
    IMapper mapper)
    : HttpRequestHandlerAsync<CartsUpdate, CartVm>
{
    public override async Task<IHttpResponse<CartVm>> HandleAsync(CartsUpdate request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.CartId)
            && (false == string.IsNullOrWhiteSpace(request.AddProductId) || false == string.IsNullOrWhiteSpace(request.RemoveProductId));
        if (!areArgumentsValid)
        {
            return Response(HttpStatusCode.BadRequest, "Invalid arguments");
        }

        var entity = await cartRepository
            .Include(it => it.Owner)
            .Include(it => it.Products)
            .ExcludeDeleted()
            .GetByIdAsync(request.CartId!, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.Gone, "Cart not found");
        }

        if (entity.Owner.Id != context.CurrentUserId)
        {
            return Response(HttpStatusCode.Gone, "You are not allowed to update this cart");
        }

        if (entity.Products.Any(it => it.Id == request.AddProductId))
        {
            return Response(HttpStatusCode.BadRequest, "Product already exists in the cart");
        }

        if (false == string.IsNullOrWhiteSpace(request.AddProductId))
        {
            var product = await productRepository.GetByIdAsync(request.AddProductId, cancellationToken);
            if (product is not null)
            {
                entity.Products.Add(product);
            }
        }

        if (false == string.IsNullOrWhiteSpace(request.RemoveProductId))
        {
            var product = entity.Products.FirstOrDefault(it => it.Id == request.RemoveProductId);
            if (product is not null)
            {
                entity.Products.Remove(product);
            }
        }

        var ack = await cartRepository.UpdateAsync(entity, cancellationToken);
        if (ack is false)
        {
            return Response(HttpStatusCode.InternalServerError, "Failed to update cart");
        }

        var vm = mapper.Map<CartVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}