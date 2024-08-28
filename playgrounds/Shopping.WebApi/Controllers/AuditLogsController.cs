using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using Shopping.WebApi.Biz.AuditLogs;
using TTSS.Core.Web.Controllers;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

namespace Shopping.WebApi.Controllers;

public sealed class AuditLogsController(IMessagingHub hub) : ApiControllerBase
{
    [HttpGet]
    public Task<ActionResult<Paging<AuditLogVm>>> Get([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 30, [FromQuery] string? keyword = default)
        => hub.SendAsync(new ListAuditLogs { PageNo = pageNo, PageSize = pageSize, Keyword = keyword }).ToActionResultAsync();
}