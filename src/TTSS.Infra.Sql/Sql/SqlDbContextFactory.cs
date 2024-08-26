using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// SqlDbContext factory.
/// </summary>
/// <remarks>
/// Initialize an instance of <see cref="SqlDbContextFactory"/>.
/// </remarks>
/// <param name="serviceProvider">The service provider</param>
public class SqlDbContextFactory(Lazy<IServiceProvider> serviceProvider) : FactoryBase(serviceProvider)
{
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

    internal IEnumerable<IInterceptor> GetInterceptors(SqlConnectionStore store, SqlInterceptorBuilder builder)
        => builder.InterceptorTypes
            .Select(it => serviceProvider.Value.GetRequiredKeyedService(it, store))
            .Cast<IInterceptor>();

    #endregion
}