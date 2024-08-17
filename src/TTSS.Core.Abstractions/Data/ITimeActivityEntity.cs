namespace TTSS.Core.Data;

/// <summary>
/// Contract for entities that have time activity traceable.
/// </summary>
public interface ITimeActivityEntity
{
    /// <summary>
    /// Created date and time of the entity.
    /// </summary>
    DateTime CreatedDate { get; set; }

    /// <summary>
    /// Last updated date and time of the entity.
    /// </summary>
    DateTime? LastUpdatedDate { get; set; }

    /// <summary>
    /// Deleted date and time of the entity.
    /// </summary>
    DateTime? DeletedDate { get; set; }
}