using Microsoft.EntityFrameworkCore;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// SqlDbContext factory.
/// </summary>
public class SqlDbContextFactory : FactoryBase
{
    #region Constructors

    /// <summary>
    /// Initialize an instance of <see cref="SqlDbContextFactory"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    public SqlDbContextFactory(Lazy<IServiceProvider> serviceProvider)
        : base(serviceProvider)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get DbContext from the given type.
    /// </summary>
    /// <param name="type">DbContext type</param>
    /// <returns>The DbContext</returns>
    /// <exception cref="ArgumentOutOfRangeException">The type must be a subclass of DbContext</exception>
    public DbContext GetDbContext(Type type)
    {
        if (!type.IsSubclassOf(typeof(DbContext)))
            throw new ArgumentOutOfRangeException($"{type.Name} must be a subclass of DbContext.");

        if (GetOrCreate(type) is DbContext ctx) return ctx;
        else throw new ArgumentOutOfRangeException($"{type.Name} isn't DbContext or forget to register.");
    }

    #endregion
}