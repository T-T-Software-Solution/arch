namespace TTSS.Core.Data;

/// <summary>
/// Entity extensions.
/// </summary>
public static class IRepositoryExtensions
{
    /// <summary>
    /// Configure the repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="config">The collection to be configured</param>
    /// <returns>The repository</returns>
    public static IRepository<TEntity> Configure<TEntity>(this IRepository<TEntity> target, Func<IQueryable<TEntity>, IQueryable<TEntity>> config)
        where TEntity : class, IDbModel<string>
    {
        Configure<TEntity, string>(target, config);
        return target;
    }

    /// <summary>
    /// Configure the repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The repository</param>
    /// <param name="config">The collection to be configured</param>
    /// <returns>The repository</returns>
    public static IRepository<TEntity, TKey> Configure<TEntity, TKey>(this IRepository<TEntity, TKey> target, Func<IQueryable<TEntity>, IQueryable<TEntity>> config)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        GetConfigurableRepository(target).Configure(config);
        return target;
    }

    /// <summary>
    /// Exclude deleted entities before querying.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The target repository</param>
    /// <returns>The repository</returns>
    public static IRepository<TEntity> ExcludeDeleted<TEntity>(this IRepository<TEntity> target)
        where TEntity : class, IDbModel<string>, ITimeActivityEntity
    {
        ExcludeDeleted<TEntity, string>(target);
        return target;
    }

    /// <summary>
    /// Exclude deleted entities before querying.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The target repository</param>
    /// <returns>The repository</returns>
    public static IRepository<TEntity, TKey> ExcludeDeleted<TEntity, TKey>(this IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>, ITimeActivityEntity
        where TKey : notnull
    {
        if (target is IConfigurableRepository<TEntity> repository)
        {
            repository.Configure(table => table.Where(it => it.DeletedDate == null));
        }

        return target;
    }

    private static IConfigurableRepository<TEntity> GetConfigurableRepository<TEntity, TKey>(IRepository<TEntity, TKey> target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        if (target is not IConfigurableRepository<TEntity> repository)
        {
            throw new NotSupportedException("The repository does not support this operation.");
        }
        return repository;
    }
}