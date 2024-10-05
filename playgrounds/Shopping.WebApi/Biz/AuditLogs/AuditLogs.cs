using Microsoft.AspNetCore.Mvc;
using Shopping.Shared.Entities.ViewModels;
using TTSS.Core.Messaging;
using TTSS.Core.Models;
using TTSS.Core.Web.Controllers;

namespace Shopping.WebApi.Biz.AuditLogs;

public sealed class AuditLogs(IMessagingHub hub) : ApiControllerBase
{
    [HttpGet]
    public Task<ActionResult<Paging<AuditLogVm>>> Get([FromQuery] int pageNo = 1,
        [FromQuery] int pageSize = 30,
        [FromQuery] IEnumerable<string>? sort = default,
        [FromQuery] Dictionary<string, string>? filter = default)
        => hub.SendAsync(new AuditLogsList { PageNo = pageNo, PageSize = pageSize, Sort = sort, Filter = filter }).ToActionResultAsync();
}