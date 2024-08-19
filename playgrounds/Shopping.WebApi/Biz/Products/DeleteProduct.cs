using Shopping.Shared.Entities;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Services;

namespace Shopping.WebApi.Biz.Products;

public sealed record DeleteProduct(string ProductId) : IRequesting;

internal sealed class DeleteProductHandler(IRepository<Product> repository, IDateTimeService dateTimeService) : RequestHandlerAsync<DeleteProduct>
{
    public override async Task HandleAsync(DeleteProduct request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.ProductId, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.DeletedDate = dateTimeService.UtcNow;
        await repository.UpdateAsync(entity, cancellationToken);
    }
}
