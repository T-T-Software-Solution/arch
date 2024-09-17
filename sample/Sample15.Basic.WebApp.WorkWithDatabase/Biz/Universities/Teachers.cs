using Microsoft.AspNetCore.Mvc;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;
using TTSS.Core.Messaging;
using TTSS.Core.Web.Controllers;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities;

public sealed class TeachersController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost]
    public Task<ActionResult<TeacherVm>> Post([FromBody] TeachersCreate request)
        => hub.SendAsync(request).ToActionResultAsync();

    [HttpPut("supervisee")]
    public Task<ActionResult> AssignSupervisee([FromBody] TeachersAssignSupervisee request)
        => hub.SendAsync(request).ToActionResultAsync();
}