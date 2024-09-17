using Microsoft.AspNetCore.Mvc;
using TTSS.Core.Messaging;
using TTSS.Core.Web.Controllers;

namespace Sample13.Basic.WebApp.ProjectStructure.Biz.Calculators;

public sealed class CalculatorsController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost("add")]
    public Task<ActionResult<double>> Add([FromBody] Add request)
        => hub.SendAsync(request).ToActionResultAsync();

    [HttpPost("sub")]
    public Task<ActionResult<double>> Sub([FromBody] Sub request)
        => hub.SendAsync(request).ToActionResultAsync();

    [HttpPost("multiply")]
    public Task<ActionResult<double>> Multiply([FromBody] Multiply request)
        => hub.SendAsync(request).ToActionResultAsync();

    [HttpPost("divide")]
    public Task<ActionResult<double>> Divide([FromBody] Divide request)
        => hub.SendAsync(request).ToActionResultAsync();
}