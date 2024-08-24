using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.Users;

public sealed record ListUsers : IHttpRequesting<IPagingResponse<UserVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public string? Keyword { get; init; }
}

internal sealed class ListUsersHandler(IRepository<User> repository)
    : HttpRequestHandlerAsync<ListUsers, IPagingResponse<UserVm>>
{
    public override async Task<IHttpResponse<IPagingResponse<UserVm>>> HandleAsync(ListUsers request, CancellationToken cancellationToken = default)
    {
        var shouldSkipSearch = string.IsNullOrEmpty(request.Keyword);
        var paging = repository.GetPaging(request.PageNo, request.PageSize,
            it => shouldSkipSearch
                || (null != it.FirstName && it.FirstName.Contains(request.Keyword!))
                || (null != it.LastName && it.LastName.Contains(request.Keyword!)));
        var vm = await paging.ToPagingDataAsync<UserVm>();
        return Response(System.Net.HttpStatusCode.OK, vm);
    }
}