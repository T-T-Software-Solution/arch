namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IRepository<TEntity, TKey> : IRepositoryBase,
    IOperationalRepository<TEntity, TKey>,
    IQueryRepository<TEntity, TKey>,
    IQueryableRepository<TEntity>,
    IDeletableRepository<TEntity, TKey>,
    IUpsertRepository<TEntity, TKey>,
    IInsertBulkRepository<TEntity>
    where TEntity : IDbModel<TKey>
    where TKey : notnull;

/// <summary>
/// Contract for repository with string primary key.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IRepository<TEntity> : IRepository<TEntity, string>
    where TEntity : IDbModel<string>;