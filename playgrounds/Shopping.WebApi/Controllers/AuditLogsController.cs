using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.AuditLogs;
using TTSS.Core.AspNetCore.Controllers;
using TTSS.Core.Messaging;

namespace Shopping.WebApi.Controllers;

public sealed class AuditLogsController(IMessagingHub hub) : ApiControllerBase
{
    [HttpGet]
    public Task<IEnumerable<AuditLogVm>> Get()
        => hub.SendAsync(new ListAuditLogs());
}