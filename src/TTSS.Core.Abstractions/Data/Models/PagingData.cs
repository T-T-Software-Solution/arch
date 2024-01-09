namespace TTSS.Core.Data.Models;

/// <summary>
/// Paging data.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public sealed class PagingData<TEntity>
{
    #region Properties

    /// <summary>
    /// Current page number.
    /// </summary>
    public required int CurrentPage { get; init; }

    /// <summary>
    /// Total entities on the current page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Total entities.
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Total pages.
    /// </summary>
    public required int PageCount { get; init; }

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
    /// Entities.
    /// </summary>
    public required IEnumerable<TEntity> Result { get; init; }

    #endregion
}