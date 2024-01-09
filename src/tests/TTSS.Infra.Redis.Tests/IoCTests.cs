using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Services;
using TTSS.Infra.Data.Redis.Caching;
using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis;

public class IoCTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var thisConfigMustBeIgnoredByCustomSetup = new RedisCacheBehavior("prefix", TimeSpan.FromHours(1));
        services
            .SetupRedisDatabase(ConnectionString)
                .AddGroup<AdvancedCache>(thisConfigMustBeIgnoredByCustomSetup)
                .AddGroup<BasicCache>()
                .AddGroup<BasicWithPrefixCache>(new("basicPrefix"))
                .AddGroup<ExpirableCache>(new(TimeSpan.FromMilliseconds(150)))
                .AddGroup<ExpirableWithPrefixCache>(new("expPrefix", TimeSpan.FromMilliseconds(100)))
            .Build();
        services.AddSingleton<IDateTimeService>(DateTimeSerivceMock.Object);
    }
}