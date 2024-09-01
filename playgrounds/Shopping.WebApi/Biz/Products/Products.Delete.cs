using Shopping.Shared.Entities;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;
using TTSS.Core.Services;

namespace Shopping.WebApi.Biz.Products;

public sealed record ProductsDelete(string ProductId) : IHttpRequesting;

file sealed class Handler(IRepository<Product> repository, IDateTimeService dateTimeService)
    : HttpRequestHandlerAsync<ProductsDelete>
{
    public override async Task<IHttpResponse> HandleAsync(ProductsDelete request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.Gone, "Product not found");
        }

        entity.DeletedDate = dateTimeService.UtcNow;
        await repository.UpdateAsync(entity, cancellationToken);
        return Response(HttpStatusCode.NoContent);
    }
}