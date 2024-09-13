using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record UsersGet(string UserId) : IHttpRequesting<UserVm>;

file class Handler(IRepository<User> repository, IMapper mapper)
    : HttpRequestHandlerAsync<UsersGet, UserVm>
{
    public override async Task<IHttpResponse<UserVm>> HandleAsync(UsersGet request, CancellationToken cancellationToken = default)
    {
        var entity = await repository
            .Query()
            .Include(it => it.Carts)
            .ThenInclude(it => it.Products)
            .FirstOrDefaultAsync(it => it.Id == request.UserId, cancellationToken);
        if (entity is null)
        {
            return Response(HttpStatusCode.Gone, "User not found");
        }

        var vm = mapper.Map<UserVm>(entity);
        return Response(HttpStatusCode.OK, vm);
    }
}