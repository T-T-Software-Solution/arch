namespace TTSS.Core.Models;

/// <summary>
/// Paging result.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public sealed class Paging<TEntity>
{
    #region Properties

    /// <summary>
    /// Total pages.
    /// </summary>
    public required int PageCount { get; init; }

    /// <summary>
    /// Current page number.
    /// </summary>
    public required int CurrentPage { get; init; }

    /// <summary>
    /// Next page number.
    /// </summary>
    public required int NextPage { get; init; }

    /// <summary>
    /// Previous page number.
    /// </summary>
    public required int PreviousPage { get; init; }

    /// <summary>
    /// Can go to next page or not.
    /// </summary>
    public required bool HasNextPage { get; init; }

    /// <summary>
    /// Can go to previous page or not.
    /// </summary>
    public required bool HasPreviousPage { get; init; }

    /// <summary>
    /// Total entities on the current page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Total entities.
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Entities.
    /// </summary>
    public required IEnumerable<TEntity> Contents { get; init; }

    #endregion
}