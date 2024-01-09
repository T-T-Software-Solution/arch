using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TTSS.Core.AspNetCore.Controllers;

/// <summary>
/// Health controller.
/// </summary>
public class HealthController : ApiControllerBase
{
    #region Methods

    /// <summary>
    /// Get current api version. 
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    [HttpHead]
    [Route("")]
    [Route("check")]
    public async Task<ActionResult<string>> GetHealth()
    {
        var healthy = await CheckAppHealth();

        if (!healthy)
            return Problem(
                "Service unhealthy",
                statusCode: (int)HttpStatusCode.ServiceUnavailable,
                title: "Health");

        return "Healthy";
    }

    /// <summary>
    /// Check if the application is healthy.
    /// </summary>
    /// <returns>True if healthy, false otherwise.</returns>
    protected virtual Task<bool> CheckAppHealth()
        => Task.FromResult(true);

    #endregion
}