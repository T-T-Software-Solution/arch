using Shopping.Shared.Entities;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record UsersDelete(string UserId) : IHttpRequesting;

file class Handler(ICorrelationContext context, IRepository<User> repository)
    : HttpRequestHandlerAsync<UsersDelete>
{
    public override async Task<IHttpResponse> HandleAsync(UsersDelete request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.UserId);
        if (!areArgumentsValid)
        {
            return Response(HttpStatusCode.BadRequest);
        }

        if (context.CurrentUserId != request.UserId)
        {
            return Response(HttpStatusCode.Forbidden);
        }

        var entity = await repository.GetByIdAsync(request.UserId, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.Gone, "User not found");
        }

        await repository.DeleteAsync(entity.Id, cancellationToken);
        return Response(HttpStatusCode.NoContent);
    }
}