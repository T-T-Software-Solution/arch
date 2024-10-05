using Shopping.Shared.Entities;
using Shopping.Shared.Entities.ViewModels;
using System.Net;
using TTSS.Core.Data;
using TTSS.Core.Facades;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Shopping.WebApi.Biz.AuditLogs;

public sealed record AuditLogsList : IHttpRequesting<Paging<AuditLogVm>>, IPagingRequest
{
    public required int PageNo { get; init; }
    public required int PageSize { get; init; }
    public IEnumerable<string>? Sort { get; init; }
    public Dictionary<string, string>? Filter { get; init; }
}

file sealed class Handler(IRepository<AuditLog> repository)
    : HttpRequestHandlerAsync<AuditLogsList, Paging<AuditLogVm>>
{
    public override async Task<IHttpResponse<Paging<AuditLogVm>>> HandleAsync(AuditLogsList request, CancellationToken cancellationToken = default)
    {
        var (order, filter) = request.ToExpressions<AuditLog>();

        var paging = await repository
            .ExcludeDeleted()
            .GetPaging(request.PageNo, request.PageSize, filter, order)
            .ExecuteAsync<AuditLogVm>();
        return Response(HttpStatusCode.OK, paging);

    }
}