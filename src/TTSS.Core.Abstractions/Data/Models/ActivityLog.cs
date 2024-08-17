using System.ComponentModel.DataAnnotations.Schema;

namespace TTSS.Core.Data.Models;

/// <summary>
/// Activity log of the entity with created, updated and deleted date and time.
/// </summary>
[ComplexType]
public class ActivityLog
{
    #region Properties

    /// <summary>
    /// Created date and time of the entity.
    /// </summary>
    public required DateTime CreatedDate { get; set; }

    /// <summary>
    /// Last updated date and time of the entity.
    /// </summary>
    public DateTime? LastUpdatedDate { get; set; }

    /// <summary>
    /// Deleted date and time of the entity.
    /// </summary>
    public DateTime? DeletedDate { get; set; }

    #endregion
}