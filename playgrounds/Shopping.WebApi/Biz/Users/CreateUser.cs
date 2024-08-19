using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.Tokens;
using Shopping.WebApi.Biz.Users.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Users;

public sealed record CreateUser : IRequesting<CreateUserResult>
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

internal sealed class CreateUserHandler(IRepository<User> repository, IMessagingHub hub, IMapper mapper) : RequestHandlerAsync<CreateUser, CreateUserResult>
{
    public override async Task<CreateUserResult> HandleAsync(CreateUser request, CancellationToken cancellationToken = default)
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
        return new CreateUserResult(vm, token);
    }
}
