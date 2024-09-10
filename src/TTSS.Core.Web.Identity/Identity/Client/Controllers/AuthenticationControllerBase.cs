using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TTSS.Core.Web.Identity.Client.Controllers;

/// <summary>
/// Authentication controller base.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public abstract class AuthenticationControllerBase : Controller
{
    #region Fields

    private const string DefaultReturnUrl = "/";

    #endregion

    #region Methods

    /// <summary>
    /// Log in with OpenID Connect.
    /// </summary>
    /// <param name="returnUrl">Return URL</param>
    /// <returns>Redirect to OpenID Connect provider</returns>
    [HttpGet("~/login")]
    public ActionResult LogIn(string returnUrl)
        => Challenge(new AuthenticationProperties
        {
            RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : DefaultReturnUrl
        }, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

    /// <summary>
    /// On receive authentication token from OpenID Connect provider.
    /// </summary>
    /// <returns>Redirect to the return URL</returns>
    /// <exception cref="InvalidOperationException">Authentication data is not authenticated</exception>
    [IgnoreAntiforgeryToken]
    [HttpGet("~/callback/login/{provider}")]
    [HttpPost("~/callback/login/{provider}")]
    public async Task<ActionResult> LogInCallback()
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true } || result.Properties is null)
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        var properties = new AuthenticationProperties(result.Properties.Items)
        {
            RedirectUri = result.Properties.RedirectUri ?? DefaultReturnUrl
        };
        properties.StoreTokens(result.Properties.GetTokens().Where(token => token switch
        {
            {
                Name:
                OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken
                or OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken
                or OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken
            } => true,
            _ => false
        }));

        var identity = CreateClaimsIdentity(new("ExternalLogin"), result.Principal ?? new ClaimsPrincipal());
        return CreateLoginResult(identity, properties) ?? Ok("Logged in successfully.");
    }

    /// <summary>
    /// Log out with OpenID Connect.
    /// </summary>
    /// <param name="returnUrl">Return URL</param>
    [HttpPost("~/logout"), ValidateAntiForgeryToken]
    public async Task<ActionResult> LogOut(string returnUrl)
    {
        var redirectUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : DefaultReturnUrl;
        var result = await HttpContext.AuthenticateAsync();
        if (result is not { Succeeded: true }) return Redirect(redirectUrl);

        await HttpContext.SignOutAsync();
        var items = new Dictionary<string, string?>
        {
            {
                OpenIddictClientAspNetCoreConstants.Properties.IdentityTokenHint,
                result.Properties.GetTokenValue(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken)
            }
        };
        var properties = new AuthenticationProperties(items) { RedirectUri = redirectUrl };
        return SignOut(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// On receive confirmation of logout from OpenID Connect provider.
    /// </summary>
    /// <returns>Redirect to the return URL</returns>
    [IgnoreAntiforgeryToken]
    [HttpGet("~/callback/logout/{provider}")]
    [HttpPost("~/callback/logout/{provider}")]
    public async Task<ActionResult> LogOutCallback()
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        return Redirect(result.Properties!.RedirectUri!);
    }

    /// <summary>
    /// Create claims identity.
    /// </summary>
    /// <param name="identity">The claims identity</param>
    /// <param name="principal">The claims principal</param>
    /// <returns>The claims identity</returns>
    protected virtual ClaimsIdentity CreateClaimsIdentity(ClaimsIdentity identity, ClaimsPrincipal principal)
    {
        string[] targetClaims = [
            Claims.Role,
            ClaimTypes.Name,
            ClaimTypes.Email,
            ClaimTypes.NameIdentifier,
            Claims.Private.RegistrationId,
        ];
        var claims = targetClaims
            .Where(principal.HasClaim)
            .Select(it => new { Name = it, Values = principal.GetClaims(it) })
            .SelectMany(it =>
            {
                List<Claim> claims = [];
                foreach (var item in it.Values)
                {
                    claims.Add(new(it.Name, item));
                }
                return claims;
            });
        identity.AddClaims(claims);
        return identity;
    }

    /// <summary>
    /// Create the login result.
    /// Return null to indicate default login message withouth sign in.
    /// </summary>
    /// <param name="identity">The claims identity</param>
    /// <param name="properties">The authentication properties</param>
    /// <returns>Login result</returns>
    protected abstract ActionResult? CreateLoginResult(ClaimsIdentity identity, AuthenticationProperties properties);

    #endregion
}