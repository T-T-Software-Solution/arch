using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TTSS.Core.Web.Identity.Server.Configurations;
using OIDD = OpenIddict.Abstractions.OpenIddictConstants;

namespace TTSS.Core.Web.Identity.Client.Configurations;

/// <summary>
/// Identity client configuration options.
/// Defaults to Scopes: 'openid email roles profile'
/// GrantTypes: 'authorization_code', and ResponseTypes: 'code'.
/// </summary>
public class IdentityClientOptions
{
    #region Fields

    private static readonly string[] DefaultScopes = [OIDD.Scopes.OpenId, OIDD.Scopes.Email, OIDD.Scopes.Roles, OIDD.Scopes.Profile];
    private static readonly string[] DefaultGrantTypes = [OIDD.GrantTypes.AuthorizationCode];
    private static readonly string[] DefaultResponseTypes = [OIDD.ResponseTypes.Code];

    #endregion

    #region Properties

    /// <summary>
    /// Enable development mode. (Default is false)
    /// If true, the application will show the detailed error page
    /// and use development certificates for encryption.
    /// </summary>
    /// <remarks>This option MUST not be enabled in production environment.</remarks>
    public bool IsDevelopmentEnabled { get; set; }

    /// <summary>
    /// The authority base URL.
    /// </summary>
    public required string AuthorityBaseUrl { get; set; }

    /// <summary>
    /// A list of URLs that the client requires.
    /// </summary>
    public IEnumerable<string>? AudienceBaseUrls { get; set; }

    /// <summary>
    /// The client identifier.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// The client secret.
    /// </summary>
    public required string ClientSecret { get; set; }

    /// <summary>
    /// The provider name.
    /// </summary>
    public required string ProviderName { get; set; }

    /// <summary>
    /// The endpoint where the client will receive the token callback.
    /// </summary>
    public string? LoginCallbackPath { get; set; } = IdentityClientRegistrarOptions.DefaultLoginCallbackPath;

    /// <summary>
    /// The endpoint where the client will receive a callback when the user logs out.
    /// </summary>
    public string? LogoutCallbackPath { get; set; } = IdentityClientRegistrarOptions.DefaultLogoutCallbackPath;

    /// <summary>
    /// The credential key for encryption and decryption.
    /// </summary>
    public Func<SymmetricSecurityKey>? CredentialKey { get; set; }

    /// <summary>
    /// Configure the bearer options.
    /// </summary>
    public Action<JwtBearerOptions>? BearerOptions { get; set; }

    /// <summary>
    /// The scopes the client is allowed to request.
    /// </summary>
    public IEnumerable<string> Scopes { get; set; } = DefaultScopes;

    /// <summary>
    /// The grant types the client is allowed to use.
    /// </summary>
    public IEnumerable<string> GrantTypes { get; set; } = DefaultGrantTypes;

    /// <summary>
    /// The response types the client is allowed to use.
    /// </summary>
    public IEnumerable<string> ResponseTypes { get; set; } = DefaultResponseTypes;

    #endregion
}