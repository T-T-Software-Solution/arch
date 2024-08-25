using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Linq.Expressions;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record ListUsers : IHttpRequesting<Paging<UserVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public string? Keyword { get; init; }
}

internal sealed class ListUsersHandler(IRepository<User> repository)
    : HttpRequestHandlerAsync<ListUsers, Paging<UserVm>>
{
    public override async Task<IHttpResponse<Paging<UserVm>>> HandleAsync(ListUsers request, CancellationToken cancellationToken = default)
    {
        var shouldSkipSearch = string.IsNullOrEmpty(request.Keyword);
        Expression<Func<User, bool>> filter = it => shouldSkipSearch
            || (null != it.FirstName && it.FirstName.Contains(request.Keyword!))
            || (null != it.LastName && it.LastName.Contains(request.Keyword!));
        var paging = await repository
            .GetPaging(request.PageNo, request.PageSize, filter)
            .ExecuteAsync<UserVm>();
        return Response(HttpStatusCode.OK, paging);
    }

}