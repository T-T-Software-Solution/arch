using TTSS.Core.Data;

namespace TTSS.Infra.Data.MongoDB;

/// <summary>
/// Contract for repository.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IMongoDbRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IDbModel<TKey>
    where TKey : notnull;

/// <summary>
/// Contract for repository with string primary key.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IMongoDbRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IDbModel<string>;