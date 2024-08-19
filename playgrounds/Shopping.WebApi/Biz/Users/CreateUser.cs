using Shopping.Shared.Entities;
using Shopping.WebApi.Biz.Tokens;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record CreateUser : IRequesting<Response>
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

internal sealed class CreateUserHandler(IRepository<User> repository, IMessagingHub hub) : RequestHandlerAsync<CreateUser, Response>
{
    public override async Task<Response> HandleAsync(CreateUser request, CancellationToken cancellationToken = default)
    {
        var entity = new User
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
        };
        await repository.InsertAsync(entity);

        return await hub.SendAsync(new CreateToken { UserId = entity.Id, FullName = $"{entity.FirstName} {entity.LastName}" });
    }
}
