namespace TTSS.Core.Models;

/// <summary>
/// Contract for paging response.
/// </summary>
/// <typeparam name="TContent">Content type</typeparam>
public interface IPagingResponse<TContent>
{
    /// <summary>
    /// Total page count.
    /// </summary>
    int PageCount { get; }

    /// <summary>
    /// Current page number.
    /// </summary>
    int CurrentPage { get; }

    /// <summary>
    /// Next page number.
    /// </summary>
    int NextPage { get; }

    /// <summary>
    /// Previous page number.
    /// </summary>
    int PreviousPage { get; }

    /// <summary>
    /// Can go to next page or not.
    /// </summary>
    bool HasNextPage { get; }

    /// <summary>
    /// Can go to previous page or not.
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    /// Total content count.
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// Contents.
    /// </summary>
    IEnumerable<TContent> Result { get; }
}