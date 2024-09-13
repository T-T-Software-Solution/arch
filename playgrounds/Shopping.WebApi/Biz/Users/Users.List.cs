using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Linq.Expressions;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record UsersList : IHttpRequesting<Paging<UserVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public string? Keyword { get; init; }
}

file sealed class Handler(IRepository<User> repository)
    : HttpRequestHandlerAsync<UsersList, Paging<UserVm>>
{
    public override async Task<IHttpResponse<Paging<UserVm>>> HandleAsync(UsersList request, CancellationToken cancellationToken = default)
    {
        var keyword = request.Keyword?.ToLower() ?? string.Empty;
        var shouldSkipSearch = string.IsNullOrEmpty(keyword);
        Expression<Func<User, bool>> filter = it => shouldSkipSearch
            || (null != it.FirstName && it.FirstName.Contains(keyword))
            || (null != it.LastName && it.LastName.Contains(keyword));
        var paging = await repository
            .Include(it => it.Carts)
            .GetPaging(request.PageNo, request.PageSize, filter)
            .ExecuteAsync<UserVm>();
        return Response(HttpStatusCode.OK, paging);
    }

}