using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// A DbContextBase instance represents a session with the database and can be used to
/// query and save instances of your entities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DbContextBase"/> class.
/// </remarks>
/// <param name="options">The options for this context</param>
public abstract class DbContextBase(DbContextOptions options) : DbContext(options)
{
    #region Fields

    private IEnumerable<IInterceptor>? _interceptors;

    #endregion

    #region Methods

    /// <summary>
    /// Configure the database (and other options) to be used for this context.
    /// </summary>
    /// <param name="optionsBuilder">A builder used to create or modify options for this context</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_interceptors?.Any() ?? false)
        {
            optionsBuilder.AddInterceptors(_interceptors);
        }
        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// Decorate the model builder with configurations from the assembly of the given type.
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        IEnumerable<Type> qry = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(it => it.PropertyType.IsGenericType && it.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(it => it.PropertyType.GetGenericArguments().FirstOrDefault()!)
            .Where(it => it is not null)
            .Where(it => typeof(IHaveActivityLog).IsAssignableFrom(it));
        foreach (var item in qry)
        {
            modelBuilder.Entity(item).ComplexProperty(nameof(IHaveActivityLog.ActivityLog)).IsRequired();
        }
        base.OnModelCreating(modelBuilder);
    }

    internal void SetInterceptors(IEnumerable<IInterceptor> interceptors)
        => _interceptors ??= interceptors;

    #endregion
}

/// <summary>
/// A DbContextBase instance represents a session with the database and can be used to
/// query and save instances of your entities.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="DbContextBase{T}"/> class.
/// </remarks>
/// <param name="options">The options for this context</param>
public abstract class DbContextBase<TEntity>(DbContextOptions<TEntity> options) : DbContextBase(options)
    where TEntity : DbContextBase<TEntity>
{
    #region Methods

    /// <summary>
    /// Decorate the model builder with configurations from the assembly of the given type.
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TEntity).Assembly);
    }

    #endregion
}