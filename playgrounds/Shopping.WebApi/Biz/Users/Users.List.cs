using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Facades;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record UsersList : IHttpRequesting<Paging<UserVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public IEnumerable<string>? Sort { get; init; }
    public Dictionary<string, string>? Filter { get; init; }
}

file sealed class Handler(IRepository<User> repository)
    : HttpRequestHandlerAsync<UsersList, Paging<UserVm>>
{
    public override async Task<IHttpResponse<Paging<UserVm>>> HandleAsync(UsersList request, CancellationToken cancellationToken = default)
    {
        var (order, filter) = request.ToExpressions<User>();

        var paging = await repository
            .GetPaging(request.PageNo, request.PageSize, filter, order)
            .ExecuteAsync<UserVm>();
        return Response(HttpStatusCode.OK, paging);
    }

}