namespace TTSS.Core.Web.Identity.Server.Configurations;

/// <summary>
/// How client interacts with the IDP.
/// </summary>
public enum ApplicationType
{
    /// <summary>
    /// Interacts with the IDP via a web browser.
    /// </summary>
    Web,

    /// <summary>
    /// Interacts with the IDP via a native application.
    /// </summary>
    Native,
}