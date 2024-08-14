namespace TTSS.Infra.Data.Sql.Models;

/// <summary>
/// Property information.
/// </summary>
public record SqlPropertyInfo
{
    /// <summary>
    /// The column name
    /// </summary>
    public required string ColumnName { get; init; }

    /// <summary>
    /// The original value
    /// </summary>
    public string? Value { get; init; }

    /// <summary>
    /// The comment of this column
    /// </summary>
    public string? Remark { get; init; }
}