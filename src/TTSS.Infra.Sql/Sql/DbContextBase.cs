using Microsoft.EntityFrameworkCore;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// A DbContextBase instance represents a session with the database and can be used to
/// query and save instances of your entities.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class DbContextBase<TEntity> : DbContext
    where TEntity : DbContextBase<TEntity>
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextBase{T}"/> class.
    /// </summary>
    /// <param name="options">The options for this context</param>
    public DbContextBase(DbContextOptions<TEntity> options) : base(options)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Decorate the model builder with configurations from the assembly of the given type.
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(TEntity).Assembly);

    #endregion
}