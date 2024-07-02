namespace TTSS.Core.Data;

/// <summary>
/// Contract for a repository that stores data in memory.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IInMemoryRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : IDbModel<TKey>
    where TKey : notnull;

/// <summary>
/// Contract for a repository that stores data in memory with string primary key.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IInMemoryRepository<TEntity> : IRepository<TEntity>
    where TEntity : IDbModel<string>;