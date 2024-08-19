using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Text.Json.Serialization;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Carts;

public sealed record UpdateCart : IRequesting<CartVm>
{
    [JsonIgnore]
    public string? CartId { get; init; }
    public string? AddProductId { get; init; }
    public string? RemoveProductId { get; init; }
}

internal sealed class UpdateCartHandler(ICorrelationContext context,
    IRepository<Cart> cartRepository,
    IRepository<Product> productRepository,
    IMapper mapper) : RequestHandlerAsync<UpdateCart, CartVm>
{
    public override async Task<CartVm> HandleAsync(UpdateCart request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.CartId)
            && (false == string.IsNullOrWhiteSpace(request.AddProductId) || false == string.IsNullOrWhiteSpace(request.RemoveProductId));
        if (!areArgumentsValid)
        {
            return null;
        }

        var entity = await cartRepository
            .Query(cancellationToken)
            .Include(it => it.Owner)
            .Include(it => it.Products)
            .FirstOrDefaultAsync(it => it.Id == request.CartId, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (entity.Owner.Id != context.CurrentUserId)
        {
            return null;
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

        await cartRepository.UpdateAsync(entity, cancellationToken);
        return mapper.Map<CartVm>(entity);
    }
}