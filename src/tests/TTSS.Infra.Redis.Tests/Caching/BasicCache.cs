namespace TTSS.Infra.Data.Redis.Caching;

internal class BasicCache : RedisCacheBase<BasicCache>
{
    public BasicCache(RedisConnectionStore connectionStore) : base(connectionStore)
    {
    }
}