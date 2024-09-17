using Microsoft.AspNetCore.Mvc;
using Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities.ViewModels;
using TTSS.Core.Messaging;
using TTSS.Core.Web.Controllers;

namespace Sample15.Basic.WebApp.WorkWithDatabase.Biz.Universities;

public class UniversitiesController(IMessagingHub hub) : ApiControllerBase
{
    [HttpGet]
    public Task<ActionResult<PersonnelListVm>> Get()
        => hub.SendAsync(new UniversitiesShowPersonnelList()).ToActionResultAsync();
}