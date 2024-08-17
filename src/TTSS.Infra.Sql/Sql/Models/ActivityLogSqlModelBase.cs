using TTSS.Core.Data.Models;

namespace TTSS.Infra.Data.Sql.Models;

/// <summary>
/// Base class for SQL entities with activity log.
/// </summary>
public abstract class ActivityLogSqlModelBase : SqlModelBase
{
    #region Properties

    /// <summary>
    /// Activity log of the entity.
    /// </summary>
    public ActivityLog ActivityLog { get; internal set; } = null!;

    #endregion
}