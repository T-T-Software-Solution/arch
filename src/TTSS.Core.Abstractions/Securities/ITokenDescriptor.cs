namespace TTSS.Core.Securities;

/// <summary>
/// Representative of the token.
/// </summary>
public interface ITokenDescriptor
{
    /// <summary>
    /// Access token.
    /// </summary>
    string? AccessToken { get; }

    /// <summary>
    /// State of the token.
    /// True if the token is valid, otherwise false.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Time when the token is available to use.
    /// Not valid before this time.
    /// </summary>
    DateTime ValidTo { get; }

    /// <summary>
    /// Time when the token is no longer available to use.
    /// Not valid after this time.
    /// </summary>
    DateTime ValidFrom { get; }

    /// <summary>
    /// User claims.
    /// </summary>
    IEnumerable<System.Security.Claims.Claim> Claims { get; }
}