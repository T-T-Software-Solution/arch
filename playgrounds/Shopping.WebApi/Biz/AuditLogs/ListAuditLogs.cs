using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Linq.Expressions;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.AuditLogs;

public sealed record ListAuditLogs : IHttpRequesting<Paging<AuditLogVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public string? Keyword { get; init; }
}

internal sealed class ListAuditLogsHandler(IRepository<AuditLog> repository)
    : HttpRequestHandlerAsync<ListAuditLogs, Paging<AuditLogVm>>
{
    public override async Task<IHttpResponse<Paging<AuditLogVm>>> HandleAsync(ListAuditLogs request, CancellationToken cancellationToken = default)
    {
        Expression<Func<AuditLog, bool>> filter = it => string.IsNullOrEmpty(request.Keyword)
            || (null != it.Description && it.Description.Contains(request.Keyword));

        var paging = await repository
            .ExcludeDeleted()
            .GetPaging(request.PageNo, request.PageSize, filter, Ordering)
            .ExecuteAsync<AuditLogVm>();
        return Response(HttpStatusCode.OK, paging);

    }
    static void Ordering(IPagingRepository<AuditLog> page)
        => page.OrderByDescending(it => it.CreatedDate);
}
