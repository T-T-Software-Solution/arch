using TTSS.Core.Data;

namespace TTSS.Infra.Data.Redis;

/// <summary>
/// Contract for Redis repository cache.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IRedisRepositoryCache<TEntity> : IRepositoryCache<TEntity>
    where TEntity : class
{
}