namespace TTSS.Core.Models;

/// <summary>
/// Contract for request with paging.
/// </summary>
public interface IPagingRequest
{
    /// <summary>
    /// Page number.
    /// </summary>
    int PageNo { get; init; }

    /// <summary>
    /// Content size per page.
    /// </summary>
    int PageSize { get; init; }

    /// <summary>
    /// Keyword for search.
    /// </summary>
    string? Keyword { get; init; }
}