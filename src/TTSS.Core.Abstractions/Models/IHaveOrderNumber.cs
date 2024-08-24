namespace TTSS.Core.Models;

/// <summary>
/// Contract for order number.
/// </summary>
public interface IHaveOrderNumber
{
    /// <summary>
    /// Order number.
    /// </summary>
    int OrderNo { get; set; }
}