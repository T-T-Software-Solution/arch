using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Text.Json.Serialization;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record UpdateUser : IRequesting<UserVm>
{
    [JsonIgnore]
    public string? UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

internal sealed class UpdateUserHandler(ICorrelationContext context, IRepository<User> repository, IMapper mapper) : RequestHandlerAsync<UpdateUser, UserVm>
{
    public override async Task<UserVm> HandleAsync(UpdateUser request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.UserId)
            && context.CurrentUserId == request.UserId
            && (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName));
        if (!areArgumentsValid)
        {
            return null;
        }

        var entity = await repository.GetByIdAsync(request.UserId, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        await repository.UpdateAsync(entity, cancellationToken);

        return mapper.Map<UserVm>(entity);
    }
}