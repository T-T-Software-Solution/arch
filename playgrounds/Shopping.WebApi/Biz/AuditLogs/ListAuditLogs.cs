using AutoMapper;
using Shipping.Shared.Entities;
using Shipping.Shared.Entities.ViewModels;
using TTSS.Core.Data;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;

namespace Shopping.WebApi.Biz.AuditLogs;

public sealed record ListAuditLogs : IRequesting<IEnumerable<AuditLogVm>>;

internal sealed class ListAuditLogsHandler(IRepository<AuditLog> repository, IMapper mapper) : RequestHandler<ListAuditLogs, IEnumerable<AuditLogVm>>
{
    public override IEnumerable<AuditLogVm> Handle(ListAuditLogs request)
        => repository.Get().Select(mapper.Map<AuditLogVm>);
}
