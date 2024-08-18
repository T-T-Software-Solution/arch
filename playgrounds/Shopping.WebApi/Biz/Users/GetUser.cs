using AutoMapper;
using Shipping.Shared.Entities;
using Shipping.Shared.Entities.ViewModels;
using System.ComponentModel.DataAnnotations;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Users;

public sealed record GetUser([Required] string UserId) : IRequesting<UserVm>;

internal class GetUserHandler(IRepository<User> repository, IMapper mapper) : RequestHandlerAsync<GetUser, UserVm>
{
    public override async Task<UserVm> HandleAsync(GetUser request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.UserId);
        if (entity is null) return null;

        return mapper.Map<UserVm>(entity);
    }
}