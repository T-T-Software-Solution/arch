using System.Security.Claims;

namespace TTSS.Core.Securities;

/// <summary>
/// Describes a raw token into a token representor.
/// </summary>
public class JsonWebTokenDescriptor : ITokenDescriptor
{
    /// <summary>
    /// Access token.
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// Time when the token is available to use.
    /// Not valid before this time.
    /// </summary>
    public DateTime ValidTo { get; init; }

    /// <summary>
    /// Time when the token is no longer available to use.
    /// Not valid after this time.
    /// </summary>
    public DateTime ValidFrom { get; init; }

    /// <summary>
    /// User claims.
    /// </summary>
    public IEnumerable<Claim> Claims { get; init; } = Enumerable.Empty<Claim>();

    /// <summary>
    /// State of the token.
    /// True if the token is valid, otherwise false.
    /// </summary>
    public virtual bool IsValid
    {
        get
        {
            var currentTime = CurrentTime?.Invoke().ToUniversalTime() ?? DateTime.UtcNow;
            var to = ValidTo.ToUniversalTime();
            var from = ValidFrom.ToUniversalTime();
            return to > currentTime && currentTime > from;
        }
    }

    /// <summary>
    /// Current time for testing purpose.
    /// </summary>
    internal Func<DateTime>? CurrentTime { get; init; }
}