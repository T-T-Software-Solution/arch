namespace TTSS.Core.Securities;

/// <summary>
/// Contract for token acknowledgment.
/// </summary>
public interface ITokenAcknowledgment
{
    /// <summary>
    /// Representative of the token.
    /// </summary>
    ITokenDescriptor? TokenDescriptor { get; }
}