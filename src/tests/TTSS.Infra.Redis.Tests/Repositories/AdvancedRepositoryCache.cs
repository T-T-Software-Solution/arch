using TTSS.Core.Data;
using TTSS.Core.Services;
using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis.Repositories;

internal class AdvancedRepositoryCache(RedisConnectionStore connectionStore,
    IDateTimeService dateTimeService) : RedisRepositoryCache<AdvancedEntity>(connectionStore)
{
    protected override RedisCacheBehavior GetCacheBehavior(RedisCacheBehavior currentBehavior)
    {
        var keyPrefix = Guid.NewGuid().ToString();
        var expirly = TimeSpan.FromMilliseconds(Config.TTL);
        return new(keyPrefix, expirly);
    }

    public async Task<AdvancedEntityPrimaryKey> SaveNewRecordAsync(int age, string name)
    {
        var id = new AdvancedEntityPrimaryKey(Guid.NewGuid().ToString());
        var entity = AdvancedEntity.Create(id, age, name, dateTimeService);
        var ack = await SetAsync(entity.Id.Value, entity);
        return ack ? entity.Id : default;
    }

    public Task<AdvancedEntity> GetRecordAsync(AdvancedEntityPrimaryKey key)
        => GetByIdAsync(key.Value);
}

public record AdvancedEntityPrimaryKey(string Value);
public record AdvancedEntity : IDbModel<AdvancedEntityPrimaryKey>
{
    public AdvancedEntityPrimaryKey Id { get; set; }
    public int Age { get; init; }
    public string Name { get; init; }
    public DateTime CreatedDate { get; init; }

    public static AdvancedEntity Create(AdvancedEntityPrimaryKey id, int age, string name, IDateTimeService dateTime)
        => new()
        {
            Id = id,
            Age = age,
            Name = name,
            CreatedDate = dateTime.UtcNow,
        };
}