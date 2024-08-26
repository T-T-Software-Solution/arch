using AutoMapper;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record GetUser(string UserId) : IHttpRequesting<UserVm>;

internal class GetUserHandler(IRepository<User> repository, IMapper mapper)
    : HttpRequestHandlerAsync<GetUser, UserVm>
{
    public override async Task<IHttpResponse<UserVm>> HandleAsync(GetUser request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(request.UserId, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.Gone, "User not found");
        }

        var vm = mapper.Map<UserVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}