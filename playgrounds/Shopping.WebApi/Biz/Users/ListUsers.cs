using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record ListUsers : IHttpRequesting<IEnumerable<UserVm>>;

internal sealed class ListUsersHandler(IRepository<User> repository, IMapper mapper) : HttpRequestHandler<ListUsers, IEnumerable<UserVm>>
{
    public override IHttpResponse<IEnumerable<UserVm>> Handle(ListUsers request)
    {
        var vm = repository.Get().Select(mapper.Map<UserVm>);
        return Response(HttpStatusCode.OK, vm);
    }
}