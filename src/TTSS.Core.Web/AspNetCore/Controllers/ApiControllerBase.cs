using Microsoft.AspNetCore.Mvc;

namespace TTSS.Core.Web.Controllers;

/// <summary>
/// Base class for API controllers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase;