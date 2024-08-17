namespace TTSS.Core.Data;

/// <summary>
/// Contract for entities that have activity log.
/// </summary>
public interface IHaveActivityLog
{
    /// <summary>
    /// Activity log of the entity.
    /// </summary>
    Models.ActivityLog ActivityLog { get; set; }
}