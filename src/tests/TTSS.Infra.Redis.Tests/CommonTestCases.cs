using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using TTSS.Infra.Data.Redis.Caching;
using TTSS.Tests;
using Xunit.Abstractions;

namespace TTSS.Infra.Data.Redis;

public abstract class CommonTestCases : IoCTestBase, IDisposable
{
    protected virtual string ConnectionString { get; } = Config.RedisConnection;

    #region Single collection

    [Fact]
    public async Task RedisCache_GetValueWhenTheKeyIsNotExisting_ShouleGetNullAndTheSystemMustNotCrash()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        var beforeSet = await sut.GetAsync(Guid.NewGuid().ToString());
        beforeSet.IsNull.Should().BeTrue();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("Hello")]
    [InlineData("tExT with WHITeSpace and CaseSENsitive")]
    [InlineData(" ")]
    [InlineData("")]
    public async Task RedisCache_SetAnyTextValueAndGetValueFromTheSameKey_ThenTheResultShouldBeTheSame(string value)
    {
        var expectedValue = value;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);
        (await sut.GetAsync(key)).ToString().Should().BeEquivalentTo(expectedValue);
    }

    [Fact]
    public async Task RedisCache_CanSetAndGetNumbericData()
    {
        var value = 999;
        var expectedValue = value;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);
        ((int)await sut.GetAsync(key)).Should().Be(expectedValue);
    }

    [Fact]
    public async Task RedisCache_CanSetAndGetBooleanData()
    {
        var value = true;
        var expectedValue = value;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);
        ((bool)await sut.GetAsync(key)).Should().Be(expectedValue);
    }

    [Fact]
    public async Task RedisCache_SetNullAsAValueAndGetIt_ThenMustGetNullAsAValueAndTheSystemMustNotCrash()
    {
        string value = null!;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);
        var actual = await sut.GetAsync(key);
        actual.IsNull.Should().BeTrue();
        actual.ToString().Should().BeEmpty();
    }

    [Fact]
    public async Task RedisCache_SetAndThenDeleteTheValueFromTheSameKey_ThenTheValueMustBeDeleted()
    {
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, Guid.NewGuid().ToString());
        await sut.DeleteAsync(key);
        var actual = await sut.GetAsync(key);
        actual.IsNull.Should().BeTrue();
        actual.ToString().Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task RedisCache_SetValueWithTheDifferenceKeys_ThenThoseValueMustBeSaveAsSeparated()
    {
        var key1 = Guid.NewGuid().ToString();
        var key2 = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();

        var value1 = Guid.NewGuid().ToString();
        await sut.SetAsync(key1, value1);

        var value2 = Guid.NewGuid().ToString();
        await sut.SetAsync(key2, value2);

        (await sut.GetAsync(key1)).ToString().Should().BeEquivalentTo(value1);
        (await sut.GetAsync(key2)).ToString().Should().BeEquivalentTo(value2);
    }

    [Fact]
    public async Task RedisCache_DeleteWithMultipleKeys_ThenThoseKeysMustBeDeleted()
    {
        var key1 = Guid.NewGuid().ToString();
        var key2 = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();

        await sut.SetAsync(key1, Guid.NewGuid().ToString());
        await sut.SetAsync(key2, Guid.NewGuid().ToString());

        await sut.DeleteAsync(new[] { key1, key2 });

        (await sut.GetAsync(key1)).IsNull.Should().BeTrue();
        (await sut.GetAsync(key2)).IsNull.Should().BeTrue();
    }

    [Fact]
    public async Task RedisCache_DeleteWithNotExistingKey_ThenTheSystemMustNotCrash()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.DeleteAsync(Guid.NewGuid().ToString());
    }

    [Fact]
    public async Task RedisCache_DeleteWithMultipleNotExistingKeys_ThenTheSystemMustNotCrash()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        var notExistingKeys = new[]
        {
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
        };
        await sut.DeleteAsync(notExistingKeys);
    }

    [Fact]
    public async Task RedisCache_DeleteValueWithTheDifferenceKeys_ThenTheMatchedKeyValueMustBeDeleted()
    {
        var key1 = Guid.NewGuid().ToString();
        var key2 = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();

        var value1 = Guid.NewGuid().ToString();
        await sut.SetAsync(key1, value1);

        var value2 = Guid.NewGuid().ToString();
        await sut.SetAsync(key2, value2);

        await sut.DeleteAsync(key1);

        (await sut.GetAsync(key1)).IsNull.Should().BeTrue();
        (await sut.GetAsync(key2)).ToString().Should().BeEquivalentTo(value2);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(100, 1, 100)]
    [InlineData(1, 5, 5)]
    [InlineData(100, 5, 500)]
    public async Task RedisCache_IncreaseValueMustBeWorkAsItBe(int rounds, int amount, int expected)
    {
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();

        while (rounds-- > 0)
        {
            await sut.IncrementAsync(key, amount);
        }

        var actual = await sut.GetAsync(key);
        ((int)actual).Should().Be(expected);
    }

    [Fact]
    public async Task RedisCache_SetValueAsAnArray_AndGetAllValueWithTheSameKey_ThenTheValueMustBeTheSame()
    {
        var savedValues = new RedisValue[] { 1, 2, 3 };
        var expectedValues = new RedisValue[] { 1, 2, 3 };
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, values: savedValues);
        (await sut.GetAsync(key, 3)).Should().BeEquivalentTo(expectedValues);
    }

    [Theory]
    [InlineData(5, 5, 5)]
    [InlineData(5, 3, 3)]
    [InlineData(5, 1, 1)]
    [InlineData(5, 0, 0)]
    public async Task RedisCache_SetValueAsAnArray_AndGetValueWithTheSameKey_ThenTheCollectionCanBeRetrieved(int totalElements, int retrieveElements, int expectedElements)
    {
        var savedValues = new RedisValue[totalElements];
        for (int i = 0; i < totalElements; i++) savedValues[i] = i;

        var expectedValues = new RedisValue[expectedElements];
        for (int i = 0; i < expectedElements; i++) expectedValues[i] = i;

        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, values: savedValues);
        (await sut.GetAsync(key, retrieveElements)).Should().BeEquivalentTo(expectedValues);
    }

    [Fact]
    public async Task RedisCache_GetArrayByInvalidRange_ThenSystemMustThrowAnException()
    {
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sut.GetAsync(key, -1));
    }

    #endregion

    [Fact]
    public async Task RedisCache_SetNullAndThenUseGenericGetCanHandleString()
    {
        string value = null!;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);

        var actualRedisValue = await sut.GetAsync(key);
        actualRedisValue.IsNull.Should().BeTrue();
        actualRedisValue.ToString().Should().BeEquivalentTo(string.Empty);

        var actualText = await sut.GetTextAsync(key);
        actualText.Should().BeEquivalentTo(value);

        var actualString = await sut.GetAsync<string>(key);
        actualString.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Hello")]
    [InlineData("Good morning!")]
    [InlineData(" SpaceFirst")]
    public async Task RedisCache_GenericGetCanHandleString(string value)
    {
        var expected = value;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);

        var actualRedisValue = await sut.GetAsync(key);
        actualRedisValue.ToString().Should().BeEquivalentTo(expected);

        var actualText = await sut.GetTextAsync(key);
        actualText.Should().BeEquivalentTo(expected);

        var actualString = await sut.GetAsync<string>(key);
        actualString.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999999)]
    [InlineData(-999999)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public async Task RedisCache_GenericGetCanHandleInteger(int value)
    {
        var expected = value;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);

        var actualRedisValue = await sut.GetAsync(key);
        actualRedisValue.Should().BeEquivalentTo(expected);

        var actualText = await sut.GetTextAsync(key);
        actualText.Should().BeEquivalentTo(expected.ToString());

        var actualNumber = await sut.GetNumberAsync<int>(key);
        actualNumber.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999999)]
    [InlineData(-999999)]
    [InlineData(0.999999)]
    [InlineData(-0.999999)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    public async Task RedisCache_GenericGetCanHandleDouble(double value)
    {
        var expected = value;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);

        var actualRedisValue = await sut.GetAsync(key);
        actualRedisValue.Should().BeEquivalentTo(expected);

        var actualText = double.Parse((await sut.GetTextAsync(key))!);
        actualText.Should().Be(expected);

        var actualNumber = await sut.GetNumberAsync<double>(key);
        actualNumber.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999999999999)]
    [InlineData(-999999999999)]
    [InlineData(0.999999999999)]
    [InlineData(-0.999999999999)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public async Task RedisCache_GenericGetCanHandleLong(long value)
    {
        var expected = value;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);

        var actualRedisValue = await sut.GetAsync(key);
        actualRedisValue.Should().BeEquivalentTo(expected);

        var actualText = double.Parse((await sut.GetTextAsync(key))!);
        actualText.Should().Be(expected);

        var actualNumber = await sut.GetNumberAsync<long>(key);
        actualNumber.Should().Be(expected);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task RedisCache_GenericGetCanHandleBoolean(bool value)
    {
        var expected = value;
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);

        var actualRedisValue = await sut.GetAsync(key);
        actualRedisValue.Should().BeEquivalentTo(expected);

        var actualText = await sut.GetTextAsync(key);
        actualText.Should().Be((expected ? 1 : 0).ToString());
    }

    [Fact]
    public Task RedisCache_GenericGetCanHandleCustomClass()
    {
        var value = new Monkey
        {
            Id = CurrentTime.Ticks,
            Name = Guid.NewGuid().ToString(),
        };
        return ValidateHandleCustomClass(value);
    }

    [Fact]
    public Task RedisCache_GenericGetCanHandleCustomClass_RefIsNotNull()
    {
        var value = new Monkey
        {
            Id = CurrentTime.Ticks,
            Name = Guid.NewGuid().ToString(),
            Ref = new Monkey
            {
                Id = CurrentTime.Ticks,
                Name = Guid.NewGuid().ToString(),
            }
        };
        return ValidateHandleCustomClass(value);
    }

    [Fact]
    public Task RedisCache_GenericGetCanHandleCustomClass_CircularReference()
    {
        var value = new Monkey
        {
            Id = CurrentTime.Ticks,
            Name = Guid.NewGuid().ToString(),
        };
        value.Ref = value;
        return ValidateHandleCustomClass(value);
    }

    private async Task ValidateHandleCustomClass(Monkey value)
    {
        var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
        var expectedText = JsonSerializer.Serialize(value, options);
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        await sut.SetAsync(key, value);

        var actualRedisValue = await sut.GetAsync(key);
        actualRedisValue.ToString().Should().BeEquivalentTo(expectedText);

        var actualText = await sut.GetTextAsync(key);
        actualText.Should().Be(expectedText);

        var actualObject = await sut.GetAsync<Monkey>(key);
        JsonSerializer.Serialize(actualObject, options).Should().BeEquivalentTo(expectedText);
    }

    [Fact]
    public async Task RedisCache_BasicWithoutKeyPrefix_MustUseDataTypeAsAKeyOnly()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        sut.Should().NotBeNull();

        var key = Guid.NewGuid().ToString();
        var value = Guid.NewGuid().ToString();
        await sut.SetAsync(key, value);

        var cache = sut as RedisCacheBase<BasicCache>;
        var database = await cache!.Redis;
        var actual = await database.StringGetAsync($"{nameof(BasicCache)}:{key}");
        actual.ToString().Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task RedisCache_BasicWithKeyPrefix_MustUseKeyPrefixFirstThenbyDataTypeAsAKeyOnly()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<BasicWithPrefixCache>>();
        sut.Should().NotBeNull();

        var key = Guid.NewGuid().ToString();
        var value = Guid.NewGuid().ToString();
        await sut.SetAsync(key, value);

        var cache = sut as RedisCacheBase<BasicWithPrefixCache>;
        var database = await cache!.Redis;
        var actual = await database.StringGetAsync($"basicPrefix:{nameof(BasicWithPrefixCache)}:{key}");
        actual.ToString().Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task RedisCache_ExpirableWithoutKeyPrefix_MustUseDataTypeAsAKeyOnly()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<ExpirableCache>>();
        sut.Should().NotBeNull();

        var key = Guid.NewGuid().ToString();
        var value = Guid.NewGuid().ToString();
        await sut.SetAsync(key, value);

        var cache = sut as RedisCacheBase<ExpirableCache>;
        var database = await cache!.Redis;
        var actual = await database.StringGetAsync($"{nameof(ExpirableCache)}:{key}");
        actual.ToString().Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task RedisCache_ExpirableWithKeyPrefix_MustUseKeyPrefixFirstThenbyDataTypeAsAKeyOnly()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<ExpirableWithPrefixCache>>();
        sut.Should().NotBeNull();

        var key = Guid.NewGuid().ToString();
        var value = Guid.NewGuid().ToString();
        await sut.SetAsync(key, value);

        var cache = sut as RedisCacheBase<ExpirableWithPrefixCache>;
        var database = await cache!.Redis;
        var actual = await database.StringGetAsync($"expPrefix:{nameof(ExpirableWithPrefixCache)}:{key}");
        actual.ToString().Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task RedisCache_AdvancedCache_MustApplyManualCacheBehaviorAsItBe()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<AdvancedCache>>();
        sut.Should().NotBeNull();

        var key = Guid.NewGuid().ToString();
        var value = Guid.NewGuid().ToString();
        await sut.SetAsync(key, value);

        var cache = sut as RedisCacheBase<AdvancedCache>;
        var database = await cache!.Redis;
        var actual = await database.StringGetAsync($"advaned-key-prefix:{nameof(AdvancedCache)}:{key}");
        actual.ToString().Should().BeEquivalentTo(value);

        await Task.Delay(200);
        (await sut.GetAsync(key)).IsNull.Should().BeTrue();
    }

    [Fact]
    public async Task RedisCache_WithExpirationTime_MustBeExpired()
    {
        var value = Guid.NewGuid().ToString();
        var key = Guid.NewGuid().ToString();
        var sut = ServiceProvider.GetRequiredService<IRedisCache<ExpirableCache>>();
        await sut.SetAsync(key, value);
        (await sut.GetAsync(key)).ToString().Should().BeEquivalentTo(value);

        await Task.Delay(200);
        (await sut.GetAsync(key)).IsNull.Should().BeTrue();
    }

    [Fact]
    public async Task RedisCache_FromInterface_WithExpirationTime_MustBeExpired()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<ExpirableCache>>();
        sut.Should().NotBeNull();

        var value = Guid.NewGuid().ToString();
        var key = Guid.NewGuid().ToString();
        await sut.SetAsync(key, value);
        (await sut.GetAsync(key)).ToString().Should().BeEquivalentTo(value);

        await Task.Delay(200);
        (await sut.GetAsync(key)).IsNull.Should().BeTrue();
    }

    [Fact]
    public void RedisCache_InterfaceRedisCaches_CanUseTheirPrimitiveObject()
    {
        var sut1 = ServiceProvider.GetRequiredService<IRedisCache<AdvancedCache>>();
        sut1.Primitive.Should().NotBeNull();
        sut1.Primitive.Should().Be(sut1);

        var sut2 = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        sut2.Primitive.Should().NotBeNull();
        sut2.Primitive.Should().Be(sut2);

        var sut3 = ServiceProvider.GetRequiredService<IRedisCache<BasicWithPrefixCache>>();
        sut3.Primitive.Should().NotBeNull();
        sut3.Primitive.Should().Be(sut3);

        var sut4 = ServiceProvider.GetRequiredService<IRedisCache<ExpirableCache>>();
        sut4.Primitive.Should().NotBeNull();
        sut4.Primitive.Should().Be(sut4);

        var sut5 = ServiceProvider.GetRequiredService<IRedisCache<ExpirableWithPrefixCache>>();
        sut5.Primitive.Should().NotBeNull();
        sut5.Primitive.Should().Be(sut5);
    }

    [Fact]
    public async Task RedisCache_InterfaceRedisCaches_CanUseTheirPrimitiveMethods_UtcTime()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<AdvancedCache>>();
        await sut.Primitive.SaveCurrentUtcTime();
        var actual = await sut.Primitive.GetSavedUtcTime();
        actual.Should().BeCloseTo(CurrentTime, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task RedisCache_InterfaceRedisCaches_CanUseTheirPrimitiveMethods_EstTime()
    {
        var sut = ServiceProvider.GetRequiredService<IRedisCache<AdvancedCache>>();
        await sut.Primitive.SaveCurrentEstTime();
        var actual = await sut.Primitive.GetSavedEstTime();
        actual.Should().BeCloseTo(CurrentEstTime, TimeSpan.FromSeconds(1));
    }

    #region Multiple collections

    [Fact]
    public async Task RedisCache_SetValueWithTheDifferenceCollections_ThenThoseValueMustBeSaveAsSeparated()
    {
        var key = Guid.NewGuid().ToString();
        var sut1 = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        var sut2 = ServiceProvider.GetRequiredService<IRedisCache<ExpirableCache>>();

        var value1 = Guid.NewGuid().ToString();
        await sut1.SetAsync(key, value1);

        var value2 = Guid.NewGuid().ToString();
        await sut2.SetAsync(key, value2);

        (await sut1.GetAsync(key)).ToString().Should().BeEquivalentTo(value1);
        (await sut2.GetAsync(key)).ToString().Should().BeEquivalentTo(value2);
    }

    [Fact]
    public async Task RedisCache_DeleteValueWithTheDifferenceCollections_ThenTheMatchedKeyValueMustBeDeleted()
    {
        var key = Guid.NewGuid().ToString();
        var sut1 = ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>();
        var sut2 = ServiceProvider.GetRequiredService<IRedisCache<ExpirableCache>>();

        var value1 = Guid.NewGuid().ToString();
        await sut1.SetAsync(key, value1);

        var value2 = Guid.NewGuid().ToString();
        await sut2.SetAsync(key, value2);

        await sut1.DeleteAsync(key);

        (await sut1.GetAsync(key)).IsNull.Should().BeTrue();
        (await sut2.GetAsync(key)).ToString().Should().BeEquivalentTo(value2);
    }

    public void Dispose()
    {
        ServiceProvider.GetRequiredService<IRedisCache<AdvancedCache>>().Dispose();
        ServiceProvider.GetRequiredService<IRedisCache<BasicCache>>().Dispose();
        ServiceProvider.GetRequiredService<IRedisCache<BasicWithPrefixCache>>().Dispose();
        ServiceProvider.GetRequiredService<IRedisCache<ExpirableCache>>().Dispose();
        ServiceProvider.GetRequiredService<IRedisCache<ExpirableWithPrefixCache>>().Dispose();
    }

    #endregion

    private class Monkey
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Monkey Ref { get; set; }
    }
}