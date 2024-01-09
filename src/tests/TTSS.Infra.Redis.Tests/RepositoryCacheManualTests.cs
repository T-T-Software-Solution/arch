using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data;
using TTSS.Core.Services;
using TTSS.Infra.Data.Redis.Models;
using TTSS.Infra.Data.Redis.Repositories;
using TTSS.Tests;
using YamlDotNet.Serialization;

namespace TTSS.Infra.Data.Redis;

public class ManualRegisterStrategy : InjectionStrategyBase
{
    public ManualRegisterStrategy(Fixture fixture, ISerializer serializer, IDeserializer deserializer, IDateTimeService dateTimeService) : base(fixture, serializer, deserializer, dateTimeService)
    {
    }

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IDateTimeService>(_ => DateTimeService);
        var store = new RedisConnectionStoreBuilder()
            .SetupDatabase(Config.RedisConnection)
                .RegisterCollection<RedisRepositoryCache<Manual_Simple>>()
                .RegisterCollection<RedisRepositoryCache<Manual_Prefix>>(new("manual-prefix-1"))
                .RegisterCollection<RedisRepositoryCache<Manual_Expiry>>(new(TimeSpan.FromMilliseconds(Config.TTL)))
                .RegisterCollection<RedisRepositoryCache<Manual_PrefixAndExpiry>>(new("manual-prefix-2", TimeSpan.FromMilliseconds(Config.TTL)))
                .RegisterCollection<AdvancedRepositoryCache>()
            .Build();
        var simpleRepositoryCache = new RedisRepositoryCache<Manual_Simple>(store);
        var simpleRepositoryCacheWithPrefix = new RedisRepositoryCache<Manual_Prefix>(store);
        var simpleRepositoryCacheWithExpiry = new RedisRepositoryCache<Manual_Expiry>(store);
        var simpleRepositoryCacheWithPrefixAndExpiry = new RedisRepositoryCache<Manual_PrefixAndExpiry>(store);
        var advancedRepositoryCache = new AdvancedRepositoryCache(store, DateTimeService);

        services
            .AddSingleton<IRepositoryCache<Manual_Simple>>(simpleRepositoryCache)
            .AddSingleton<IRepositoryCache<Manual_Prefix>>(simpleRepositoryCacheWithPrefix)
            .AddSingleton<IRepositoryCache<Manual_Expiry>>(simpleRepositoryCacheWithExpiry)
            .AddSingleton<IRepositoryCache<Manual_PrefixAndExpiry>>(simpleRepositoryCacheWithPrefixAndExpiry)
            .AddSingleton<IRepositoryCache<AdvancedEntity>>(advancedRepositoryCache);
    }
}

public class Manual_SimpleRepositoryCacheTests : RepositoryCacheCommonTestCases<ManualRegisterStrategy, Manual_Simple> { }
public class Manual_SimpleRepositoryCacheWithPrefixTests : RepositoryCacheCommonTestCases<ManualRegisterStrategy, Manual_Prefix> { }
public class Manual_SimpleRepositoryCacheWithExpiryTests : RepositoryCacheWithExpiryTestsBase<ManualRegisterStrategy, Manual_Expiry> { }
public class Manual_SimpleRepositoryCacheWithPrefixAndExpiryTests : RepositoryCacheWithExpiryTestsBase<ManualRegisterStrategy, Manual_PrefixAndExpiry> { }
public class Manual_AdvancedRepositoryCacheTests : AdvancedRepositoryCacheTestsBase<ManualRegisterStrategy, AdvancedEntity> { }