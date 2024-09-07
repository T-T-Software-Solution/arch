using System.Security.Claims;
using System.Text.Json.Serialization;
using TTSS.Core.Models;

namespace TTSS.Core.Web.Models;

/// <summary>
/// Default for web correlation context.
/// </summary>
public class WebCorrelationContext : CorrelationContext, IHaveIdentity
{
    #region Properties

    /// <summary>
    /// Current HTTP context.
    /// </summary>
    [JsonIgnore]
    public HttpContext? HttpContext { get; internal set; }

    /// <summary>
    /// User identity.
    /// </summary>
    [JsonIgnore]
    public ClaimsPrincipal? User => HttpContext?.User;

    #endregion

    #region Methods

    internal void SetHttpContext(HttpContext httpContext)
        => HttpContext = httpContext;

    #endregion
}