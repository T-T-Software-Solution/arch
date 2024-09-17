using Microsoft.AspNetCore.Mvc;
using Sample15.Basic.WebApp.WorkWithDatabase.Entities.ViewModels;
using TTSS.Core.Messaging;
using TTSS.Core.Web.Controllers;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities;

public sealed class StudentsController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost]
    public Task<ActionResult<StudentVm>> Post([FromBody] StudentsCreate request)
        => hub.SendAsync(request).ToActionResultAsync();

    [HttpDelete("{id}")]
    public Task<ActionResult> Delete(string id, [FromQuery] bool isHardDelete = false)
        => hub.SendAsync(new StudentsDelete(id, isHardDelete)).ToActionResultAsync();
}