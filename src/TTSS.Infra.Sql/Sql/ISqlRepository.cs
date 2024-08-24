using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// Contract for repository.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface ISqlRepository<TEntity, TKey> : IRepository<TEntity, TKey>,
    ISqlRepositorySpecific<TEntity>,
    IConfigurableRepository<TEntity>,
    IAsyncQueryRepository<TEntity, TKey>,
    IDisposable,
    IAsyncDisposable
    where TEntity : class, IDbModel<TKey>
    where TKey : notnull;

/// <summary>
/// Contract for repository with string primary key.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface ISqlRepository<TEntity> : IRepository<TEntity>,
    ISqlRepository<TEntity, string>
    where TEntity : class, IDbModel<string>;