using Microsoft.Extensions.DependencyInjection;
using TTSS.Infra.Data.Redis.Caching;
using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis;

public class ManualTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var thisConfigMustBeIgnoredByCustomSetup = new RedisCacheBehavior("prefix", TimeSpan.FromHours(1));
        var store = new RedisConnectionStoreBuilder()
            .SetupDatabase(ConnectionString)
                .RegisterCollection<AdvancedCache>(thisConfigMustBeIgnoredByCustomSetup)
                .RegisterCollection<BasicCache>()
                .RegisterCollection<BasicWithPrefixCache>(new("basicPrefix"))
                .RegisterCollection<ExpirableCache>(new(TimeSpan.FromMilliseconds(150)))
                .RegisterCollection<ExpirableWithPrefixCache>(new("expPrefix", TimeSpan.FromMilliseconds(100)))
           .Build();
        var advancedCache = new AdvancedCache(store, DateTimeSerivceMock.Object);
        var basicCache = new BasicCache(store);
        var basicWithPrefixCache = new BasicWithPrefixCache(store);
        var expirableCache = new ExpirableCache(store);
        var expirableWithPrefixCache = new ExpirableWithPrefixCache(store);

        services
            .AddSingleton<IRedisCache<AdvancedCache>>(advancedCache)
            .AddSingleton<IRedisCache<BasicCache>>(basicCache)
            .AddSingleton<IRedisCache<BasicWithPrefixCache>>(basicWithPrefixCache)
            .AddSingleton<IRedisCache<ExpirableCache>>(expirableCache)
            .AddSingleton<IRedisCache<ExpirableWithPrefixCache>>(expirableWithPrefixCache);
    }
}