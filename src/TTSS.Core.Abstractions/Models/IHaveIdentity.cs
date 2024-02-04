namespace TTSS.Core.Models;

/// <summary>
/// Contract for a model that has an identity.
/// </summary>
public interface IHaveIdentity
{
    /// <summary>
    /// User identity.
    /// </summary>
    System.Security.Claims.ClaimsPrincipal? User { get; }
}