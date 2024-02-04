using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace TTSS.Core.Models;

/// <summary>
/// Helper extensions for <see cref="System.Security.Claims.ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Check if the user has all the roles.
    /// </summary>
    /// <param name="target">The ClaimsPrincipal</param>
    /// <param name="roles">Roles to check</param>
    /// <returns>Returns true if the user has all the roles, otherwise false.</returns>
    public static bool HasAllRoles([NotNullWhen(true)] this ClaimsPrincipal? target, params string[] roles)
        => target is not null && roles.All(target.IsInRole);

    /// <summary>
    /// Check if the user has any of the roles.
    /// </summary>
    /// <param name="target">The ClaimsPrincipal</param>
    /// <param name="roles">Roles to check</param>
    /// <returns>Returns true if the user has any of the roles, otherwise false.</returns>
    public static bool HasAnyRolesOf([NotNullWhen(true)] this ClaimsPrincipal? target, params string[] roles)
        => target is not null && roles.Any(target.IsInRole);

    /// <summary>
    /// Check if the user is authenticated.
    /// </summary>
    /// <param name="target">The ClaimsPrincipal</param>
    /// <returns>Returns true if the user is authenticated, otherwise false.</returns>
    public static bool IsAuthenticated([NotNullWhen(true)] this ClaimsPrincipal? target)
        => target is not null
        && target.Identity is not null
        && target.Identity.IsAuthenticated
        && target.FindAll(ClaimTypes.NameIdentifier).Any();

    /// <summary>
    /// Get the user ID from the ClaimsPrincipal.
    /// </summary>
    /// <param name="target">The ClaimsPrincipal</param>
    /// <returns>Returns the user ID if found, otherwise null.</returns>
    public static string? GetUserId(this ClaimsPrincipal? target)
        => target?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>
    /// Get the user ID from the ClaimsPrincipal.
    /// </summary>
    /// <param name="target">The ClaimsPrincipal</param>
    /// <returns>Returns the user ID if found, otherwise throw an exception.</returns>
    /// <exception cref="InvalidOperationException">User ID in the ClaimsPrincipal not found</exception>
    public static string RequireUserIdAndThrowExceptionWhenNotFound(this ClaimsPrincipal? target)
        => target?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID in the ClaimsPrincipal not found");
}