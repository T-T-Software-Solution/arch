using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace TTSS.Core.Web.Identity.Server.Configurations;

/// <summary>
/// Entra ID (Azure AD) external authentication provider options.
/// </summary>
public class EntraIdOptions
{
    /// <summary>
    /// Enable Entra ID external authentication. (Default is false)
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The Azure AD tenant ID (e.g., "contoso.onmicrosoft.com" or GUID).
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// The application (client) ID from Azure AD app registration.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The client secret from Azure AD app registration.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// The instance URL for Azure AD. (Default is https://login.microsoftonline.com/)
    /// </summary>
    public string Instance { get; set; } = "https://login.microsoftonline.com/";

    /// <summary>
    /// The callback path for Entra ID authentication. (Default is /signin-entra-id)
    /// </summary>
    public string CallbackPath { get; set; } = "/signin-entra-id";

    /// <summary>
    /// The sign-out callback path. (Default is /signout-callback-entra-id)
    /// </summary>
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-entra-id";

    /// <summary>
    /// The remote sign-out path. (Default is /signout-entra-id)
    /// </summary>
    public string RemoteSignOutPath { get; set; } = "/signout-entra-id";

    /// <summary>
    /// Additional configuration for OpenIdConnect options.
    /// </summary>
    public Action<OpenIdConnectOptions>? ConfigureOptions { get; set; }

    /// <summary>
    /// Gets the authority URL for Entra ID.
    /// </summary>
    public string GetAuthority() => $"{Instance.TrimEnd('/')}/{TenantId}";
}
