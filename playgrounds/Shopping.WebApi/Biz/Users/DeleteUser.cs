using Shipping.Shared.Entities;
using System.ComponentModel.DataAnnotations;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record DeleteUser([Required] string UserId) : IRequesting;

internal class DeleteUserHandler(ICorrelationContext context, IRepository<User> repository) : RequestHandlerAsync<DeleteUser>
{
    public override async Task HandleAsync(DeleteUser request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.UserId)
            && context.CurrentUserId == request.UserId;
        if (!areArgumentsValid) return;

        var entity = await repository.GetByIdAsync(request.UserId);
        if (entity is null) return;

        await repository.DeleteAsync(entity.Id);
    }
}