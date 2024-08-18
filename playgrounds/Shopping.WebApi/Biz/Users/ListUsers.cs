using AutoMapper;
using Shipping.Shared.Entities;
using Shipping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.Users;

public sealed record ListUsers : IRequesting<IEnumerable<UserVm>>;

internal sealed class ListUsersHandler(IRepository<User> repository, IMapper mapper) : RequestHandler<ListUsers, IEnumerable<UserVm>>
{
    public override IEnumerable<UserVm> Handle(ListUsers request)
    {
        return repository.Get().Select(mapper.Map<UserVm>);
    }
}