using TTSS.Core.Services;
using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis.Caching;

internal class AdvancedCache : RedisCacheBase<AdvancedCache>
{
    private const string CurrentTimeKey = "current:time";
    private readonly IDateTimeService _dateTimeService;

    public AdvancedCache(RedisConnectionStore connectionStore,
        IDateTimeService dateTimeService) : base(connectionStore)
        => _dateTimeService = dateTimeService;

    protected override RedisCacheBehavior GetCacheBehavior(RedisCacheBehavior currentBehavior)
    {
        var keyPrefix = "advaned-key-prefix";
        var expirly = TimeSpan.FromMilliseconds(100);
        return new(keyPrefix, expirly);
    }

    public async Task SaveCurrentUtcTime()
    {
        var currentTime = _dateTimeService.UtcNow;
        var timeText = _dateTimeService.ToNumericDateTime(currentTime);
        await SetAsync(CurrentTimeKey, timeText);
    }

    public async Task<DateTime?> GetSavedUtcTime()
    {
        var savedTimeText = await GetTextAsync(CurrentTimeKey);
        return string.IsNullOrWhiteSpace(savedTimeText)
            ? default
            : _dateTimeService.ParseNumericToUtcDateTime(savedTimeText);
    }

    public async Task SaveCurrentEstTime()
    {
        var estTime = _dateTimeService.EstNow;
        var currentTime = _dateTimeService.ToUtcTime(estTime);
        var timeText = _dateTimeService.ToNumericDateTime(currentTime);
        await SetAsync(CurrentTimeKey, timeText);
    }

    public async Task<DateTime?> GetSavedEstTime()
    {
        var savedTimeText = await GetTextAsync(CurrentTimeKey);

        var utcTime = string.IsNullOrWhiteSpace(savedTimeText)
            ? default
            : _dateTimeService.ParseNumericToUtcDateTime(savedTimeText);
        var estTime = _dateTimeService.ToEstTime(utcTime);
        return estTime;
    }
}