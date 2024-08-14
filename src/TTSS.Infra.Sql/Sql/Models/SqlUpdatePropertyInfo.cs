namespace TTSS.Infra.Data.Sql.Models;

/// <summary>
/// Property information for update.
/// </summary>
public sealed record SqlUpdatePropertyInfo : SqlPropertyInfo
{
    /// <summary>
    /// The new value
    /// </summary>
    public string? NewValue { get; init; }
}