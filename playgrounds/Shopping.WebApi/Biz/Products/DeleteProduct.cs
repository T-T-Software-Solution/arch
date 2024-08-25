using Shopping.Shared.Entities;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;
using TTSS.Core.Services;

namespace Shopping.WebApi.Biz.Products;

public sealed record DeleteProduct(string ProductId) : IHttpRequesting;

internal sealed class DeleteProductHandler(IRepository<Product> repository, IDateTimeService dateTimeService)
    : HttpRequestHandlerAsync<DeleteProduct>
{
    public override async Task<IHttpResponse> HandleAsync(DeleteProduct request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.NotFound);
        }

        entity.DeletedDate = dateTimeService.UtcNow;
        await repository.UpdateAsync(entity, cancellationToken);
        return Response(HttpStatusCode.NoContent);
    }
}