namespace TTSS.Core.Web.Identity.Server.Configurations;

/// <summary>
/// How the client manages credentials.
/// </summary>
public enum ClientType
{
    /// <summary>
    /// Client can not securely store credentials.
    /// Like a mobile applications, SPA, or desktop applications.
    /// </summary>
    Public,

    /// <summary>
    /// Client can securely store credentials.
    /// Like a server-side web application.
    /// </summary>
    Confidential,
}