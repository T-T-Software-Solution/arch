using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography.X509Certificates;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TTSS.Core.Web.Identity.Server.Configurations;

/// <summary>
/// Identity server configuration options.
/// By default, AccessToken will expire in 1 hour and RefreshToken will expire in 8 hours.
/// And challenge lifetime is 2 minutes.
/// </summary>
public class IdentityServerOptions
{
    #region Fields

    internal const int DefaultChallengeLifetime = 2;
    internal const int DefaultAccessTokenLifetime = 1;
    internal const int DefaultRefreshTokenLifetime = 8;
    internal static readonly string[] DefaultScopes = [Scopes.OpenId, Scopes.Email, Scopes.Roles, Scopes.Profile];

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
    /// The supported scopes.
    /// (Default is "openid email roles profile")
    /// </summary>
    public IEnumerable<string> SupportedScopes { get; set; } = DefaultScopes;

    /// <summary>
    /// The certificate for signing the tokens.
    /// </summary>
    /// <remarks>This property is required for production environment.</remarks>
    public Func<X509Certificate2>? Certificate { get; set; }

    /// <summary>
    /// The challenge lifetime in seconds. (Default is 2 minutes)
    /// </summary>
    public TimeSpan ChallengeLifetime { get; set; } = TimeSpan.FromMinutes(DefaultChallengeLifetime);

    /// <summary>
    /// The access token lifetime in seconds. (Default is 1 hour)
    /// </summary>
    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromHours(DefaultAccessTokenLifetime);

    /// <summary>
    /// The refresh token lifetime in seconds. (Default is 8 hours)
    /// </summary>
    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromHours(DefaultRefreshTokenLifetime);

    /// <summary>
    /// Setups the cookie authentication options.
    /// </summary>
    public Action<CookieAuthenticationOptions>? CookieAuthenticationOptions { get; set; }

    /// <summary>
    /// Setups the identity options.
    /// </summary>
    public Action<IdentityOptions>? IdentityOptions { get; set; }

    /// <summary>
    /// Setups the identity builder.
    /// </summary>
    public Action<IdentityBuilder>? IdentityBuilder { get; set; }

    /// <summary>
    /// Setups the OpenIddict core builder.
    /// </summary>
    public Action<OpenIddictCoreBuilder>? OpenIddictCoreBuilder { get; set; }

    /// <summary>
    /// Setups the OpenIddict server builder.
    /// </summary>
    public Action<OpenIddictServerBuilder>? OpenIddictServerBuilder { get; set; }

    /// <summary>
    /// Setups the OpenIddict validation builder.
    /// </summary>
    public Action<OpenIddictValidationBuilder>? OpenIddictValidationBuilder { get; set; }

    #endregion

    #region Methods

    internal static TimeSpan GetDuration(TimeSpan? value, TimeSpan defaultValue)
        => (value is not null && value.Value > TimeSpan.Zero) ? value.Value : defaultValue;

    #endregion
}