namespace TTSS.Core.Securities;

/// <summary>
/// Describes a raw token into a token representor.
/// </summary>
public interface ITokenDescriber
{
    /// <summary>
    /// Create a token representor from a token.
    /// </summary>
    /// <param name="token">Access token</param>
    /// <returns>Token representor</returns>
    ITokenDescriptor? Describe(string? token);
}