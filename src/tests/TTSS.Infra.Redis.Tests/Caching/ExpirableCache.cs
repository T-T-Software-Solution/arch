namespace TTSS.Infra.Data.Redis.Caching;

internal class ExpirableCache : RedisCacheBase<ExpirableCache>
{
    public ExpirableCache(RedisConnectionStore connectionStore) : base(connectionStore)
    {
    }
}