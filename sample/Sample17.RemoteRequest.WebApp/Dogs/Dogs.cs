using Microsoft.AspNetCore.Mvc;
using TTSS.Core.Messaging;
using TTSS.Core.Web.Controllers;

namespace Sample17.RemoteRequest.WebApp.Dogs;

public sealed class DogsController(IMessagingHub hub) : ApiControllerBase
{
    [HttpPost]
    public Task<ActionResult> Post([FromBody] DogsCreate request)
        => hub.SendAsync(request).ToActionResultAsync();
}