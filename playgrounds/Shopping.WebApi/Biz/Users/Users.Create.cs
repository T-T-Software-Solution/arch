using AutoMapper;
using MassTransit;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using Shopping.Shared.Requests;
using Shopping.WebApi.Biz.Tokens;
using Shopping.WebApi.Biz.Users.ViewModels;
using System.Net;
using TTSS.Core.Annotations;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

[OperationDescription(OperationType.Create, "User", "Create a new user.")]
public sealed record UsersCreate : IHttpRequesting<CreateUserResult>
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

file sealed class Handler(IRepository<User> repository, IMessagingHub hub, IMapper mapper, IBus bus)
    : HttpRequestHandlerAsync<UsersCreate, CreateUserResult>
{
    public override async Task<IHttpResponse<CreateUserResult>> HandleAsync(UsersCreate request, CancellationToken cancellationToken = default)
    {
        var entity = new User
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
        };
        await repository.InsertAsync(entity, cancellationToken);

        var vm = mapper.Map<UserVm>(entity);
        var token = await hub.SendAsync(new CreateToken
        {
            UserId = entity.Id,
            FullName = $"{entity.FirstName} {entity.LastName}",
        }, cancellationToken);
        var result = new CreateUserResult(vm, token);

        await bus.Publish(new UserRegistered
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
        });

        return Response(HttpStatusCode.OK, result);
    }
}