using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using TTSS.Core.Web.Identity.Server.Configurations;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TTSS.Core.Web.Identity.Server.Controllers;

/// <summary>
/// Authorization controller base.
/// </summary>
/// <typeparam name="TUser">Identity user type</typeparam>
/// <param name="options">Identity configuration options</param>
/// <param name="userManager">User manager</param>
/// <param name="signInManager">Sign-in manager</param>
public abstract class AuthorizationControllerBase<TUser>(IOptions<IdentityServerOptions> options,
    UserManager<TUser> userManager,
    SignInManager<TUser> signInManager)
    : Controller where TUser : IdentityUser
{
    #region Methods

    /// <summary>
    /// Exchange token.
    /// </summary>
    /// <exception cref="InvalidOperationException">The OpenID Connect request cannot be retrieved.</exception>
    [HttpPost("~/connect/token")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        ClaimsPrincipal? claimsPrincipal = null;

        if (request.IsClientCredentialsGrantType())
        {
            // Client credentials flow
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

            // Add extra calims here (don't forget to add destination otherwise it won't be added to the access token)
            // identity.AddClaim("some-claim", "some-value", Destinations.AccessToken);

            claimsPrincipal = new ClaimsPrincipal(identity);
            claimsPrincipal.SetScopes(request.GetScopes());
        }
        else if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            // Authorization code flow
            var authResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            if (authResult is null)
            {
                return ErrorResponse(Errors.InsufficientAccess, "The authorization code is not valid.");
            }
            claimsPrincipal = authResult.Principal;
        }

        if (claimsPrincipal is null)
        {
            return ErrorResponse(Errors.UnsupportedGrantType, "The specified grant type is not supported.");
        }

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        IActionResult ErrorResponse(string error, string message)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = error,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = message,
            });
            return Challenge(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    /// <summary>
    /// Authroize.
    /// </summary>
    [IgnoreAntiforgeryToken]
    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request is null)
        {
            ModelState.AddModelError("Request", "The OpenID Connect request cannot be retrieved.");
            return BadRequest(ModelState);
        }

        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (false == (User?.Identity?.IsAuthenticated ?? false) || false == result.Succeeded || string.IsNullOrWhiteSpace(result.Principal?.Identity?.Name))
        {
            var querystring = QueryString.Create(Request.HasFormContentType ? [.. Request.Form] : Request.Query.ToList());
            var expiry = DateTimeOffset.UtcNow.Add(IdentityServerOptions.GetDuration(options?.Value?.ChallengeLifetime,
                TimeSpan.FromMinutes(IdentityServerOptions.DefaultChallengeLifetime)));
            var properties = new AuthenticationProperties
            {
                RedirectUri = $"{Request.PathBase}{Request.Path}{querystring}",
                ExpiresUtc = expiry,
            };
            return Challenge(properties, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        var userId = User.GetClaim(Claims.Subject)!;
        List<Claim> claims = [new Claim(Claims.Subject, userId)];

        AssignRoleClaim(User, claims);

        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        claimsPrincipal.SetScopes(request.GetScopes());
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        static void AssignRoleClaim(ClaimsPrincipal user, IList<Claim> claims)
        {
            var roles = user.FindAll(Claims.Role) ?? [];
            foreach (var item in roles)
            {
                var roleClaim = new Claim(Claims.Role, item.Value)
                    .SetDestinations(Destinations.AccessToken);
                claims.Add(roleClaim);
            }
        }
    }

    /// <summary>
    /// Get user information.
    /// </summary>
    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/connect/userinfo")]
    [Produces("application/json")]
    public async Task<IActionResult> Userinfo()
    {
        if (false == TryGetUserId(out var userId)) return ErrorResponse();
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return ErrorResponse();

        var claims = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            [Claims.Subject] = await userManager.GetUserIdAsync(user)
        };

        if (User.HasScope(Scopes.Email))
        {
            claims[Claims.Email] = await userManager.GetEmailAsync(user);
            claims[Claims.EmailVerified] = await userManager.IsEmailConfirmedAsync(user);
        }
        if (User.HasScope(Scopes.Phone))
        {
            claims[Claims.PhoneNumber] = await userManager.GetPhoneNumberAsync(user);
            claims[Claims.PhoneNumberVerified] = await userManager.IsPhoneNumberConfirmedAsync(user);
        }
        if (User.HasScope(Scopes.Roles))
        {
            claims[Claims.Role] = await userManager.GetRolesAsync(user);
        }

        SetExtraUserInfo(User, claims);

        return Ok(claims);

        bool TryGetUserId([NotNullWhen(true)] out string? userId)
        {
            userId = User.GetClaim(Claims.Subject);
            return !string.IsNullOrEmpty(userId);
        }
        IActionResult ErrorResponse()
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is bound to an account that no longer exists."
            });
            return Challenge(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    /// <summary>
    /// Logout.
    /// </summary>
    [HttpGet("~/connect/logout")]
    public async Task<IActionResult> Logout()
    {
        const string LogoutRedirectKey = "post_logout_redirect_uri";
        var returnUrl = Request.Query
            .TryGetValue(LogoutRedirectKey, out var returnUrlValue) ? returnUrlValue.FirstOrDefault() : null;

        if (User?.Identity?.IsAuthenticated == true)
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
        }

        const string ReturnUrlKey = "returnUrl";
        if (string.IsNullOrWhiteSpace(returnUrl)
            && Request.Query.TryGetValue(ReturnUrlKey, out var newReturnUrl))
        {
            returnUrl = newReturnUrl;
        }

        return SignOut(new AuthenticationProperties { RedirectUri = returnUrl },
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Set extra user information.
    /// </summary>
    /// <param name="claims">User claims</param>
    /// <param name="userInfo">Current user information</param>
    protected abstract void SetExtraUserInfo(ClaimsPrincipal claims, IDictionary<string, object?> userInfo);

    #endregion
}

/// <summary>
/// Authorization controller base.
/// </summary>
/// <param name="options">Identity configuration options</param>
/// <param name="userManager">User manager</param>
/// <param name="signInManager">Sign-in manager</param>
public abstract class AuthorizationControllerBase(IOptions<IdentityServerOptions> options,
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager)
    : AuthorizationControllerBase<IdentityUser>(options, userManager, signInManager);