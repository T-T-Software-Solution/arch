using Microsoft.AspNetCore.Mvc;
using Sample16.RemoteRequest.Shared.ViewModels;
using TTSS.Core.Messaging;
using TTSS.Core.Web.Controllers;

namespace Sample17.RemoteRequest.WebApp.Pings;

public sealed class PingsController(IMessagingHub hub) : ApiControllerBase
{
    [HttpGet]
    public Task<ActionResult<PongVm>> Get([FromQuery] int first = 3, [FromQuery] int second = 7)
        => hub.SendAsync(new PingsGet(first, second)).ToActionResultAsync();
}