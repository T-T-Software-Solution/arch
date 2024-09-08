using Microsoft.AspNetCore.Identity;

namespace TTSS.Core.Web.Identity.Server.Models;

/// <summary>
/// Represents a request to register a new account.
/// </summary>
/// <typeparam name="TIdentityUser">The type of the user to register</typeparam>
/// <param name="User">A user to register</param>
/// <param name="Password">A password for the user</param>
/// <param name="Roles">A collection of roles to assign to the user</param>
public sealed record RegisterAccount<TIdentityUser>(TIdentityUser User, string Password, IEnumerable<string>? Roles)
    where TIdentityUser : IdentityUser;