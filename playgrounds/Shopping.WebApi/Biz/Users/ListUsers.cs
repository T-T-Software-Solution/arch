using AutoMapper;
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

internal sealed class ListUsersHandler(IRepository<User> repository, IMapper mapper)
    : HttpRequestHandlerAsync<ListUsers, IPagingResponse<UserVm>>
{
    public override async Task<IHttpResponse<IPagingResponse<UserVm>>> HandleAsync(ListUsers request, CancellationToken cancellationToken = default)
    {
        var shouldSkipSearch = string.IsNullOrEmpty(request.Keyword);
        var paging = await repository.GetPagingAsync<User, UserVm>(it =>
            shouldSkipSearch || (it.FirstName.Contains(request.Keyword) || it.LastName.Contains(request.Keyword)), d => d, request, mapper, cancellationToken);
        return Response(System.Net.HttpStatusCode.OK, paging);
    }
}