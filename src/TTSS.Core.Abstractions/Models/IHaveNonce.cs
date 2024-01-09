namespace TTSS.Core.Models;

/// <summary>
/// Contract for a model that has a nonce.
/// </summary>
public interface IHaveNonce
{
    /// <summary>
    /// Number used once.
    /// </summary>
    string Nonce { get; }
}