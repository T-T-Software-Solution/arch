namespace TTSS.Core.Data;

/// <summary>
/// Contract for entities that have user activity traceable.
/// </summary>
public interface IUserActivityEntity
{
    /// <summary>
    /// User ID who created the entity.
    /// </summary>
    string CreatedById { get; set; }

    /// <summary>
    /// User ID who updated the entity.
    /// </summary>
    string? LastUpdatedById { get; set; }

    /// <summary>
    /// User ID who deleted the entity.
    /// </summary>
    string? DeletedById { get; set; }
}