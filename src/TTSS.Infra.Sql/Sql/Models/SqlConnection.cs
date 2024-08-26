using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql.Models;

/// <summary>
/// SQK connection information.
/// </summary>
public sealed record SqlConnection
{
    #region Properties

    /// <summary>
    /// Data type name.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// DbContext data type.
    /// </summary>
    public Type DbContextDataType { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnection"/> class.
    /// </summary>
    /// <param name="entityType">Type of collection</param>
    /// <param name="dbContextType">Type of DbContext</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The collection type must be a subclass of SqlModelBase.
    /// The dbContext type must be a subclass of DbContext.
    /// </exception>
    public SqlConnection(Type entityType, Type dbContextType)
    {
        if (false == (entityType?.IsAssignableTo(typeof(IDbModel)) ?? false))
        {
            throw new ArgumentOutOfRangeException($"{nameof(entityType)} must implement IDbModel.");
        }

        if (false == (dbContextType?.IsSubclassOf(typeof(DbContext)) ?? false))
        {
            throw new ArgumentOutOfRangeException($"{nameof(DbContextDataType)} must be a subclass of DbContext.");
        }

        TypeName = entityType.Name;
        DbContextDataType = dbContextType;
    }

    #endregion
}