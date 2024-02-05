using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// Contract for repository.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface ISqlRepository<TEntity, TKey> : IRepository<TEntity, TKey>,
    ISqlRepositorySpecific<TEntity>,
    IAsyncQueryRepository<TEntity, TKey>,
    IDisposable,
    IAsyncDisposable
    where TEntity : IDbModel<TKey>
    where TKey : notnull;

/// <summary>
/// Contract for repository with string primary key.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface ISqlRepository<TEntity> : IRepository<TEntity>,
    ISqlRepositorySpecific<TEntity>,
    IAsyncQueryRepository<TEntity, string>,
    IDisposable,
    IAsyncDisposable
    where TEntity : IDbModel<string>;