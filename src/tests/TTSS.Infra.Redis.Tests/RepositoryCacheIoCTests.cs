using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Services;
using TTSS.Infra.Data.Redis.Models;
using TTSS.Infra.Data.Redis.Repositories;
using TTSS.Tests;
using YamlDotNet.Serialization;

namespace TTSS.Infra.Data.Redis;

public class IoCStrategy : InjectionStrategyBase
{
    public IoCStrategy(Fixture fixture, ISerializer serializer, IDeserializer deserializer, IDateTimeService dateTimeService) : base(fixture, serializer, deserializer, dateTimeService)
    {
    }

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IDateTimeService>(_ => DateTimeService);
        services
            .SetupRedisDatabase(Config.RedisConnection)
                .AddRepositoryCache<RedisRepositoryCache<IoC_Simple>>()
                .AddRepositoryCache<RedisRepositoryCache<IoC_Prefix>>(new("ioc-prefix-1"))
                .AddRepositoryCache<RedisRepositoryCache<IoC_Expiry>>(new(TimeSpan.FromMilliseconds(Config.TTL)))
                .AddRepositoryCache<RedisRepositoryCache<IoC_PrefixAndExpiry>>(new("ioc-prefix-2", TimeSpan.FromMilliseconds(Config.TTL)))
                .AddRepositoryCache<AdvancedRepositoryCache>()
            .Build();
    }
}

public class IoC_SimpleRepositoryCacheTests : RepositoryCacheCommonTestCases<IoCStrategy, IoC_Simple> { }
public class IoC_SimpleRepositoryCacheWithPrefixTests : RepositoryCacheCommonTestCases<IoCStrategy, IoC_Prefix> { }
public class IoC_SimpleRepositoryCacheWithExpiryTests : RepositoryCacheWithExpiryTestsBase<IoCStrategy, IoC_Expiry> { }
public class IoC_SimpleRepositoryCacheWithPrefixAndExpiryTests : RepositoryCacheWithExpiryTestsBase<IoCStrategy, IoC_PrefixAndExpiry> { }
public class IoC_AdvancedRepositoryCacheTests : AdvancedRepositoryCacheTestsBase<IoCStrategy, AdvancedEntity> { }