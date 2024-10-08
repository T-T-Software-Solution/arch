﻿using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using System.Text.Json.Serialization;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record UsersUpdate : IHttpRequesting<UserVm>
{
    [JsonIgnore]
    public string? UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

file sealed class Handler(ICorrelationContext context, IRepository<User> repository, IMapper mapper)
    : HttpRequestHandlerAsync<UsersUpdate, UserVm>
{
    public override async Task<IHttpResponse<UserVm>> HandleAsync(UsersUpdate request, CancellationToken cancellationToken = default)
    {
        var areArgumentsValid = !string.IsNullOrWhiteSpace(request.UserId)
            && (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName));
        if (!areArgumentsValid)
        {
            return Response(HttpStatusCode.BadRequest, "Invalid arguments");
        }

        if (context.CurrentUserId != request.UserId)
        {
            return Response(HttpStatusCode.Forbidden, "You are not allowed to update this user");
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