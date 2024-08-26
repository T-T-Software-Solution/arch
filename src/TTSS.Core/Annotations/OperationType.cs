namespace TTSS.Core.Annotations;

/// <summary>
/// Type of operation.
/// </summary>
public enum OperationType
{
    /// <summary>
    /// Unknown operation.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Read operation.
    /// </summary>
    Read = 0,

    /// <summary>
    /// List operation.
    /// Same as <see cref="Read"/> but for multiple items.
    /// </summary>
    List = Read,

    /// <summary>
    /// Create operation.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Update operation.
    /// </summary>
    Update = 2,

    /// <summary>
    /// Delete operation.
    /// </summary>
    Delete = 4,
}