using Microsoft.EntityFrameworkCore;
using Shopping.Shared.Entities;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;
using TTSS.Core.Services;

namespace Shopping.WebApi.Biz.Carts;

public sealed record DeleteCart(string CartId) : IRequesting;

internal class DeleteCartHandler(ICorrelationContext context,
    IRepository<Cart> repository,
    IDateTimeService dateTimeService) : RequestHandlerAsync<DeleteCart>
{
    public override async Task HandleAsync(DeleteCart request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.CartId);
        if (!areArgumentsValid)
        {
            return;
        }

        var entity = await repository
            .Query()
            .Include(it => it.Owner)
            .FirstOrDefaultAsync(it => it.Id == request.CartId);
        if (entity is null)
        {
            return;
        }

        if (entity.Owner.Id != context.CurrentUserId)
        {
            return;
        }

        entity.DeletedDate = dateTimeService.UtcNow;
        await repository.UpdateAsync(entity);
    }
}