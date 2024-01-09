namespace TTSS.Infra.Data.Redis.Caching;

internal class ExpirableWithPrefixCache : RedisCacheBase<ExpirableWithPrefixCache>
{
    public ExpirableWithPrefixCache(RedisConnectionStore connectionStore) : base(connectionStore)
    {
    }
}