using OIDD = OpenIddict.Abstractions.OpenIddictConstants;

namespace TTSS.Core.Web.Identity.Server.Configurations;

/// <summary>
/// Identity client registrar options.
/// Defaults to Scopes: 'openid email roles profile'
/// </summary>
public class IdentityClientRegistrarOptions
{
    #region Fields

    internal const string DefaultLoginCallbackPath = "callback/login/local";
    internal const string DefaultLogoutCallbackPath = "callback/logout/local";
    private static readonly string[] DefaultRequirements = ["pkce"];
    private static readonly string[] DefaultAllowEndpoints = ["token", "logout", "authorization"];
    private static readonly string[] DefaultAllowGrantTypes = [OIDD.GrantTypes.RefreshToken, OIDD.GrantTypes.AuthorizationCode, OIDD.GrantTypes.ClientCredentials];
    private static readonly string[] DefaultAllowResponseTypes = [OIDD.ResponseTypes.Code];

    #endregion

    #region Properties

    /// <summary>
    /// The application identifier.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// The application display name.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// The application secret.
    /// </summary>
    public required string ClientSecret { get; set; }

    /// <summary>
    /// How the client manages credentials.
    /// </summary>
    public ClientType ClientType { get; set; } = ClientType.Confidential;

    /// <summary>
    /// How client interacts with the IDP.
    /// </summary>
    public ApplicationType ApplicationType { get; set; } = ApplicationType.Web;

    /// <summary>
    /// The application requirements.
    /// </summary>
    public IEnumerable<string> Requirements { get; set; } = DefaultRequirements;

    /// <summary>
    /// The endpoints where the application will receive the token callback.
    /// </summary>
    public required IEnumerable<string> LoginCallbackEndpoints { get; set; } = [];

    /// <summary>
    /// The endpoints where the application will receive a callback when the user logs out.
    /// </summary>
    public required IEnumerable<string> LogoutCallbackEndpoints { get; set; } = [];

    /// <summary>
    /// The scopes the application is allowed to request.
    /// </summary>
    public IEnumerable<string> AllowScopes { get; set; } = IdentityServerOptions.DefaultScopes;

    /// <summary>
    /// The endpoints the application is allowed to access.
    /// </summary>
    public IEnumerable<string> AllowEndpoints { get; set; } = DefaultAllowEndpoints;

    /// <summary>
    /// The grant types the application is allowed to use.
    /// </summary>
    public IEnumerable<string> AllowGrantTypes { get; set; } = DefaultAllowGrantTypes;

    /// <summary>
    /// The response types the application is allowed to use.
    /// </summary>
    public IEnumerable<string> AllowResponseTypes { get; set; } = DefaultAllowResponseTypes;

    #endregion

    #region Methods

    /// <summary>
    /// Create the login callback URL.
    /// </summary>
    /// <param name="clientBaseUrl">Base URL of the client application</param>
    /// <returns>The login callback URL</returns>
    public static string CreateLoginCallbackPath(string clientBaseUrl)
        => new UriBuilder(clientBaseUrl) { Path = DefaultLoginCallbackPath }.Uri.AbsoluteUri;

    /// <summary>
    /// Create the logout callback URL.
    /// </summary>
    /// <param name="clientBaseUrl">Base URL of the client application</param>
    /// <returns>The logout callback URL</returns>
    public static string CreateLogoutCallbackPath(string clientBaseUrl)
        => new UriBuilder(clientBaseUrl) { Path = DefaultLogoutCallbackPath }.Uri.AbsoluteUri;

    #endregion
}