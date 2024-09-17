using Microsoft.AspNetCore.Mvc;
using Sample12.Basic.WebApp.ChainRequest.Messages;
using TTSS.Core.Messaging;
using TTSS.Core.Web.Controllers;

namespace Sample12.Basic.WebApp.ChainRequest.Controllers;

public sealed class CalculatorsController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost]
    public Task<ActionResult<double>> Get([FromBody] CalculateRequest request)
        => hub.SendAsync(request).ToActionResultAsync();
}