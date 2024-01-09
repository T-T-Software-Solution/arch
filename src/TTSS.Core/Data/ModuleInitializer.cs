using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Core.Data;

/// <summary>
/// Helper extension methods for register in-memory repository.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Registers in-memory repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterInMemoryRepository<TEntity, TKey>(this IServiceCollection target)
        where TEntity : IDbModel<TKey>
        where TKey : notnull
        => target.AddSingleton<IRepository<TEntity, TKey>>(_ => new InMemoryRepository<TEntity, TKey>(it => it.Id));

    /// <summary>
    /// Registers in-memory repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterInMemoryRepository<TEntity>(this IServiceCollection target)
        where TEntity : IDbModel<string>
        => target.AddSingleton<IRepository<TEntity>>(_ => new InMemoryRepository<TEntity>(it => it.Id));
}