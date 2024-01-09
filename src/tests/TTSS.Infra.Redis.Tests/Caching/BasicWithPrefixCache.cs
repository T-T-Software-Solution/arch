namespace TTSS.Infra.Data.Redis.Caching;

internal class BasicWithPrefixCache : RedisCacheBase<BasicWithPrefixCache>
{
    public BasicWithPrefixCache(RedisConnectionStore connectionStore) : base(connectionStore)
    {
    }
}