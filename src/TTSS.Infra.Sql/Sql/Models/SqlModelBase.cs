using System.ComponentModel.DataAnnotations;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql.Models;

/// <summary>
/// Base class for SQL entities.
/// </summary>
public abstract class SqlModelBase : IDbModel<string>, IValidatableEntity
{
    #region Properties

    /// <summary>
    /// Primary key of the entity.
    /// </summary>
    [Key]
    public required string Id { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Check if the entity is in a valid state.
    /// </summary>
    /// <returns>True if the entity is in a valid state.</returns>
    public virtual bool IsValidState()
        => false == string.IsNullOrWhiteSpace(Id);

    #endregion
}