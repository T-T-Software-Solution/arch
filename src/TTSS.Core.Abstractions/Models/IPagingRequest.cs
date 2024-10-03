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
    /// Sort fields.
    /// Default is ascending.
    /// </summary>
    /// <example>
    /// [ "name", "age:asc", "year:desc" ]
    /// </example>
    IEnumerable<string>? Sort { get; init; }

    /// <summary>
    /// Filter fields by specific values.
    /// </summary>
    /// <example>
    /// [ {"name","John"}, {"age","30"} ]
    /// </example>
    Dictionary<string, string>? Filter { get; init; }
}