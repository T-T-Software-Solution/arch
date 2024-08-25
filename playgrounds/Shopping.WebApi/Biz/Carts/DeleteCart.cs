using Microsoft.EntityFrameworkCore;
using Shopping.Shared.Entities;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;
using TTSS.Core.Services;

namespace Shopping.WebApi.Biz.Carts;

public sealed record DeleteCart(string CartId) : IHttpRequesting;

internal class DeleteCartHandler(ICorrelationContext context,
    IRepository<Cart> repository,
    IDateTimeService dateTimeService)
    : HttpRequestHandlerAsync<DeleteCart>
{
    public override async Task<IHttpResponse> HandleAsync(DeleteCart request, CancellationToken cancellationToken = default)
    {
        if (context.CurrentUserId is null)
        {
            return Response(HttpStatusCode.Unauthorized);
        }

        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.CartId);
        if (!areArgumentsValid)
        {
            return Response(HttpStatusCode.BadRequest, "Invalid arguments");
        }

        var entity = await repository
            .Query(cancellationToken)
            .Include(it => it.Owner)
            .FirstOrDefaultAsync(it => it.Id == request.CartId, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.NotFound, "Cart not found");
        }

        if (entity.Owner.Id != context.CurrentUserId)
        {
            return Response(HttpStatusCode.Forbidden);
        }

        entity.DeletedDate = dateTimeService.UtcNow;
        await repository.UpdateAsync(entity, cancellationToken);
        return Response(HttpStatusCode.NoContent);
    }
}