using Shipping.Shared.Entities;
using Shopping.WebApi.Biz.Tokens;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Users;

public sealed record CreateUser : IRequesting<string>
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

internal sealed class CreateUserHandler(IRepository<User> repository, IMessagingHub hub) : RequestHandlerAsync<CreateUser, string>
{
    public override async Task<string> HandleAsync(CreateUser request, CancellationToken cancellationToken = default)
    {
        var entity = new User
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
        };
        await repository.InsertAsync(entity);

        return await hub.SendAsync(new CreateToken { UserId = entity.Id });
    }
}
