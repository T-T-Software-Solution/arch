namespace TTSS.Infra.Data.Sql.Models;

/// <summary>
/// Update information of entity.
/// </summary>
public sealed record PropertyUpdateInfo
{
    /// <summary>
    /// The column name
    /// </summary>
    public required string ColumnName { get; init; }

    /// <summary>
    /// Before value changed
    /// </summary>
    public string? OriginalValue { get; init; }

    /// <summary>
    /// After value changed
    /// </summary>
    public string? NewValue { get; init; }

    /// <summary>
    /// The comment of this column
    /// </summary>
    public string? Remark { get; init; }
}