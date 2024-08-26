using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using System.Text.Json.Serialization;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record UpdateUser : IHttpRequesting<UserVm>
{
    [JsonIgnore]
    public string? UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

internal sealed class UpdateUserHandler(ICorrelationContext context, IRepository<User> repository, IMapper mapper)
    : HttpRequestHandlerAsync<UpdateUser, UserVm>
{
    public override async Task<IHttpResponse<UserVm>> HandleAsync(UpdateUser request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.UserId)
            && context.CurrentUserId == request.UserId
            && (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName));
        if (!areArgumentsValid)
        {
            return Response(HttpStatusCode.BadRequest, "Invalid arguments");
        }

        var entity = await repository.GetByIdAsync(request.UserId!, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.Gone, "User not found");
        }

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        var ack = await repository.UpdateAsync(entity, cancellationToken);
        if (ack is false)
        {
            return Response(HttpStatusCode.InternalServerError, "Failed to update user");
        }

        var vm = mapper.Map<UserVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}