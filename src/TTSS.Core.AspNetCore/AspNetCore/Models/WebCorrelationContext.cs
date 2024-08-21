using System.Text.Json.Serialization;
using TTSS.Core.Models;

namespace TTSS.Core.AspNetCore.Models;

/// <summary>
/// Default for web correlation context.
/// </summary>
public class WebCorrelationContext : CorrelationContext
{
    #region Properties

    /// <summary>
    /// Current HTTP context.
    /// </summary>
    [JsonIgnore]
    public HttpContext? HttpContext { get; internal set; }

    #endregion

    #region Methods

    internal void SetHttpContext(HttpContext httpContext)
        => HttpContext = httpContext;

    #endregion
}