using Microsoft.AspNetCore.Mvc;
using Sample11.Basic.WebApp.SetupProject.Messages;
using TTSS.Core.Messaging;
using TTSS.Core.Web.Controllers;

namespace Sample11.Basic.WebApp.SetupProject.Controllers;

public sealed class DemosController(IMessagingHub hub) : ApiControllerBase
{
    [HttpGet("greeting/{message}")]
    public Task<ActionResult> Greeting(string message)
        => hub.SendAsync(new Greeting { Message = message }).ToActionResultAsync();

    [HttpGet("ping/{name}")]
    public Task<ActionResult<Pong>> Ping(string name)
        => hub.SendAsync(new Ping { Name = name }).ToActionResultAsync();
}