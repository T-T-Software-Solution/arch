namespace TTSS.Core.Models;

/// <summary>
/// Represents a paging response.
/// </summary>
public abstract record PagingResponse : IPagingResponse
{
    #region Properties

    /// <summary>
    /// Total page count.
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Next page number.
    /// </summary>
    public int NextPage { get; set; }

    /// <summary>
    /// Previous page number.
    /// </summary>
    public int PreviousPage { get; set; }

    /// <summary>
    /// Can go to next page or not.
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Can go to previous page or not.
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Total content count.
    /// </summary>
    public int TotalCount { get; set; }

    #endregion

    #region Methods

    public static PagingResponse<TContent> Empty<TContent>() => new();

    #endregion
}

/// <summary>
/// Represents a paging response.
/// </summary>
/// <typeparam name="TContent">Content type</typeparam>
public record PagingResponse<TContent> : PagingResponse, IPagingResponse<TContent>
{
    #region Properties

    /// <summary>
    /// Contents.
    /// </summary>
    public IEnumerable<TContent> Data { get; set; } = [];

    #endregion
}