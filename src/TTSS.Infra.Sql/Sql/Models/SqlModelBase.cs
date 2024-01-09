using System.ComponentModel.DataAnnotations;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql.Models;

/// <summary>
/// Base class for SQL entities.
/// </summary>
public abstract class SqlModelBase : IDbModel<string>
{
    #region Properties

    /// <summary>
    /// Primary key of the entity.
    /// </summary>
    [Key]
    public required string Id { get; set; }

    #endregion
}