using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TTSS.Core.Web.Identity.Server.Controllers;

/// <summary>
/// External authentication controller base for handling external login providers like Entra ID.
/// </summary>
/// <typeparam name="TUser">Identity user type</typeparam>
/// <param name="userManager">User manager</param>
/// <param name="signInManager">Sign-in manager</param>
public abstract class ExternalAuthenticationControllerBase<TUser>(
    UserManager<TUser> userManager,
    SignInManager<TUser> signInManager)
    : Controller where TUser : IdentityUser, new()
{
    /// <summary>
    /// Initiates external login with specified provider (e.g., "EntraId").
    /// </summary>
    /// <param name="provider">The authentication provider name</param>
    /// <param name="returnUrl">The URL to return to after authentication</param>
    [HttpGet("~/connect/external-login")]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), new { returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    /// <summary>
    /// Handles the callback from external authentication provider.
    /// </summary>
    /// <param name="returnUrl">The URL to return to after authentication</param>
    /// <param name="remoteError">Any error from the remote provider</param>
    [HttpGet("~/connect/external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!string.IsNullOrEmpty(remoteError))
        {
            return await HandleExternalLoginError(remoteError, returnUrl);
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return await HandleExternalLoginError("Error loading external login information.", returnUrl);
        }

        // Sign in the user with this external login provider if the user already has a login
        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            return await HandleExternalLoginError("User account is locked out.", returnUrl);
        }

        // If the user does not have an account, create one
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            return await HandleExternalLoginError("Email claim not received from external provider.", returnUrl);
        }

        var user = await userManager.FindByEmailAsync(email);
        var isNewUser = user == null;

        if (isNewUser)
        {
            user = await CreateExternalUserAsync(info);
            var createResult = await userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return await HandleExternalLoginError($"Error creating user: {errors}", returnUrl);
            }

            // Call hook after user is created
            await OnUserCreatedAsync(user, info);
        }

        // Add external login to the user
        var addLoginResult = await userManager.AddLoginAsync(user!, info);
        if (!addLoginResult.Succeeded)
        {
            var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
            return await HandleExternalLoginError($"Error adding external login: {errors}", returnUrl);
        }

        // Add claim to track external authentication scheme for federated sign-out
        var externalSchemeClaim = new Claim("external_scheme", info.LoginProvider);
        await userManager.AddClaimAsync(user!, externalSchemeClaim);

        // Sign in the user
        await signInManager.SignInAsync(user!, isPersistent: false, info.LoginProvider);

        return LocalRedirect(returnUrl);
    }

    /// <summary>
    /// Creates a new user from external login information.
    /// Override this method to customize user creation logic.
    /// </summary>
    /// <param name="info">External login info</param>
    /// <returns>The created user</returns>
    protected virtual Task<TUser> CreateExternalUserAsync(ExternalLoginInfo info)
    {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;

        var user = new TUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true // External providers verify email
        };

        // Set additional user properties if available
        if (user is IdentityUser identityUser && !string.IsNullOrEmpty(name))
        {
            // Store name in a way that's accessible
            identityUser.UserName = email;
        }

        OnUserCreating(user, info);

        return Task.FromResult(user);
    }

    /// <summary>
    /// Called when creating a new user from external login.
    /// Override to add custom claims or user properties.
    /// </summary>
    /// <param name="user">The user being created</param>
    /// <param name="info">External login information</param>
    protected virtual void OnUserCreating(TUser user, ExternalLoginInfo info)
    {
        // Override in derived class to customize
    }

    /// <summary>
    /// Called after a new user has been created from external login.
    /// Override to add roles, claims, or perform other post-creation tasks.
    /// </summary>
    /// <param name="user">The created user</param>
    /// <param name="info">External login information</param>
    protected virtual Task OnUserCreatedAsync(TUser user, ExternalLoginInfo info)
    {
        // Override in derived class to customize
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles external login errors.
    /// Override this method to customize error handling.
    /// </summary>
    /// <param name="error">The error message</param>
    /// <param name="returnUrl">The return URL</param>
    /// <returns>Action result for error handling</returns>
    protected virtual Task<IActionResult> HandleExternalLoginError(string error, string returnUrl)
    {
        // Default: redirect to login page with error
        var loginUrl = $"/identity/account/login?error={Uri.EscapeDataString(error)}&returnUrl={Uri.EscapeDataString(returnUrl)}";
        return Task.FromResult<IActionResult>(Redirect(loginUrl));
    }
}

/// <summary>
/// External authentication controller base with default IdentityUser.
/// </summary>
/// <param name="userManager">User manager</param>
/// <param name="signInManager">Sign-in manager</param>
public abstract class ExternalAuthenticationControllerBase(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager)
    : ExternalAuthenticationControllerBase<IdentityUser>(userManager, signInManager);
