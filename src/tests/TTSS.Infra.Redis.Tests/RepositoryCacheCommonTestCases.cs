using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data;
using TTSS.Infra.Data.Redis.Repositories;
using TTSS.Tests;

namespace TTSS.Infra.Data.Redis;

public abstract class RepositoryCacheCommonTestCases<TInjectionStrategy, TEntity> : IoCTestBase<TInjectionStrategy>,
    IDisposable
    where TInjectionStrategy : InjectionStrategyBase
    where TEntity : class
{
    protected IRepositoryCache<TEntity> Sut => ServiceProvider.GetRequiredService<IRepositoryCache<TEntity>>();

    [Fact]
    public void Resolve_IRepositoryCache_MustBeResolved()
        => Sut.Should().NotBeNull();

    [Fact]
    public void Resolve_IRedisRepositoryCache_MustBeResolved()
        => Sut.Should().NotBeNull();

    [Fact]
    public async Task SetAsync_MustBeWorkCorrectly()
    {
        var id = Fixture.Create<string>();
        var data = Fixture.Create<TEntity>();
        (await Sut.SetAsync(id, data)).Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_MustBeWorkCorrectly()
    {
        var id = Fixture.Create<string>();
        var data = Fixture.Create<TEntity>();
        (await Sut.SetAsync(id, data)).Should().BeTrue();
        (await Sut.GetByIdAsync(id)).Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task GetAsync_MustBeWorkCorrectly()
    {
        var insertData = Fixture
            .CreateMany<string>(10)
            .Select(it => new { Id = it, Data = Fixture.Create<TEntity>() })
            .ToList();
        foreach (var item in insertData)
        {
            (await Sut.SetAsync(item.Id, item.Data)).Should().BeTrue();
        }
        var actual = await Sut.GetAsync(insertData.Select(it => it.Id));
        actual.Should().NotBeNull().And.BeEquivalentTo(insertData.Select(it => it.Data));
    }

    [Fact]
    public async Task GetByIdAsync_WhenTheIdIsNotExisting_ThenReceivedNull()
        => (await Sut.GetByIdAsync("Unknown")).Should().BeNull();

    [Theory]
    [InlineData("simple")]
    [InlineData(" s p a c e")]
    [InlineData(@"!@#$%^&*()_+[]{}\|/~`")]
    public async Task SetAsync_WithAnyIdType_MustBeWorkCorrectly(string id)
    {
        var data = Fixture.Create<TEntity>();
        (await Sut.SetAsync(id, data)).Should().BeTrue();
        (await Sut.GetByIdAsync(id)).Should().BeEquivalentTo(data);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task SetAsync_WithInvalidKeyValue_ThenOperationMustBeIgnored(string id)
    {
        (await Sut.SetAsync(id, Fixture.Create<TEntity>())).Should().BeFalse();
        (await Sut.GetByIdAsync(id)).Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithNullValue_TheNullMustBeSetted()
    {
        var id = Fixture.Create<string>();
        (await Sut.SetAsync(id, null)).Should().BeTrue();
        (await Sut.GetByIdAsync(id)).Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_OnTheSameId_ThePreviousValueMustBeReplacedByTheNewest()
    {
        var id = Fixture.Create<string>();
        var firstValue = Fixture.Create<TEntity>();
        var secondValue = Fixture.Create<TEntity>();
        firstValue.Should().NotBe(secondValue).And.NotBeEquivalentTo(secondValue);

        (await Sut.SetAsync(id, firstValue)).Should().BeTrue();
        (await Sut.SetAsync(id, secondValue)).Should().BeTrue();
        (await Sut.GetByIdAsync(id)).Should().BeEquivalentTo(secondValue);
    }

    [Fact]
    public async Task GetAsync_WhenTheIdIsNotExisting_ThenReceivedNull()
    {
        var actual = await Sut.GetAsync(Fixture.CreateMany<string>(10));
        actual.Should().NotBeNull().And.AllSatisfy(it => it.Should().BeNull());
    }

    [Fact]
    public async Task DeleteAsync_MustBeWorkCorrectly()
    {
        var id = Fixture.Create<string>();
        var data = Fixture.Create<TEntity>();
        (await Sut.SetAsync(id, data)).Should().BeTrue();
        (await Sut.DeleteAsync(id)).Should().BeTrue();
        (await Sut.GetByIdAsync(id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_MustBeDeleteTheSpecificIdOnly()
    {
        var firstId = Fixture.Create<string>();
        var firstData = Fixture.Create<TEntity>();
        (await Sut.SetAsync(firstId, firstData)).Should().BeTrue();

        var secondId = Fixture.Create<string>();
        var secondData = Fixture.Create<TEntity>();
        (await Sut.SetAsync(secondId, secondData)).Should().BeTrue();

        (await Sut.DeleteAsync(firstId)).Should().BeTrue();
        (await Sut.GetByIdAsync(firstId)).Should().BeNull();
        (await Sut.GetByIdAsync(secondId)).Should().NotBeNull().And.BeEquivalentTo(secondData);
    }

    [Fact]
    public async Task DeleteManyAsync_MustBeWorkCorrectly()
    {
        var insertData = Fixture.CreateMany<string>(10)
            .Select(it => new
            {
                Id = it,
                Data = Fixture.Create<TEntity>()
            })
            .ToList();
        var tasks = insertData.Select(it => Sut.SetAsync(it.Id, it.Data));
        Task.WaitAll(tasks.ToArray());
        tasks.All(it => it.Result).Should().BeTrue();

        var ids = insertData.Select(it => it.Id);
        (await Sut.DeleteManyAsync(ids)).Should().BeTrue();
        (await Sut.GetAsync(ids)).Should().NotBeNull().And.AllSatisfy(it => it.Should().BeNull());
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task DeleteAsync_WhenTheIdIsNotExisting_ThenTheAcknowledgementMustBeFalse(string id)
        => (await Sut.DeleteAsync(id)).Should().BeFalse();

    [Fact]
    public async Task Counter_MustWorkCorrectly()
    {
        (await Sut.IncrementCounterAsync()).Should().Be(1);
        (await Sut.IncrementCounterAsync()).Should().Be(2);
        (await Sut.IncrementCounterAsync()).Should().Be(3);
        (await Sut.IncrementCounterAsync()).Should().Be(4);

        (await Sut.DecrementCounterAsync()).Should().Be(3);
        (await Sut.DecrementCounterAsync()).Should().Be(2);
        (await Sut.DecrementCounterAsync()).Should().Be(1);
        (await Sut.DecrementCounterAsync()).Should().Be(0);

        (await Sut.IncrementCounterAsync(5)).Should().Be(5);
        (await Sut.IncrementCounterAsync(10)).Should().Be(15);
        (await Sut.IncrementCounterAsync(100)).Should().Be(115);
        (await Sut.IncrementCounterAsync(500)).Should().Be(615);

        (await Sut.DecrementCounterAsync(500)).Should().Be(115);
        (await Sut.DecrementCounterAsync(100)).Should().Be(15);
        (await Sut.DecrementCounterAsync(10)).Should().Be(5);

        (await Sut.ResetCounter()).Should().BeTrue();
        (await Sut.IncrementCounterAsync()).Should().Be(1);
        (await Sut.DecrementCounterAsync()).Should().Be(0);

        (await Sut.DecrementCounterAsync(-10)).Should().Be(10);
        (await Sut.IncrementCounterAsync(-10)).Should().Be(0);
    }

    public void Dispose()
    {
        Sut.Dispose();
        GC.SuppressFinalize(this);
    }
}

public abstract class RepositoryCacheWithExpiryTestsBase<TInjectionStrategy, TEntity> : RepositoryCacheCommonTestCases<TInjectionStrategy, TEntity>
    where TInjectionStrategy : InjectionStrategyBase
    where TEntity : class
{
    [Fact]
    public async Task GetByIdAsync_AfterExpired_TheValueMustBeNull()
    {
        var id = Fixture.Create<string>();
        var data = Fixture.Create<TEntity>();
        (await Sut.SetAsync(id, data)).Should().BeTrue();

        await Config.WaitUntilExpired;

        (await Sut.GetByIdAsync(id)).Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_AfterExpired_TheValueMustBeNull()
    {
        var insertData = Fixture.CreateMany<string>(10)
            .Select(it => new { Id = it, Data = Fixture.Create<TEntity>() })
            .ToList();
        var tasks = insertData.Select(it => Sut.SetAsync(it.Id, it.Data));
        Task.WaitAll(tasks.ToArray());
        tasks.All(it => it.Result).Should().BeTrue();

        await Config.WaitUntilExpired;

        var actualTasks = insertData.Select(it => Sut.GetByIdAsync(it.Id));
        Task.WaitAll(actualTasks.ToArray());
        actualTasks
            .Select(it => it.Result)
            .Should().AllSatisfy(it => it.Should().BeNull());
    }

    [Fact(Skip = "TBD, conflict with other tests.")]
    public async Task SetCounter_ThenWaitUntilExpired_ThenTheValueMustBeResetted()
    {
        (await Sut.IncrementCounterAsync(100)).Should().Be(100);
        await Config.WaitUntilExpired;
        (await Sut.IncrementCounterAsync(5)).Should().Be(5);
    }
}

public abstract class AdvancedRepositoryCacheTestsBase<TInjectionStrategy, TEntity> : RepositoryCacheWithExpiryTestsBase<TInjectionStrategy, TEntity>
    where TInjectionStrategy : InjectionStrategyBase
    where TEntity : class
{
    [Fact]
    public void Cast_IRepositoryCache_ToPrimitiveType_ShouldBeWorkCorrectly()
        => (Sut as AdvancedRepositoryCache).Should().NotBeNull();

    [Fact]
    public async Task IRepositoryCache_MustBeAccessToThePrimitiveMethods()
    {
        var sut = Sut as AdvancedRepositoryCache;
        var age = Fixture.Create<int>();
        var name = Fixture.Create<string>();
        var key = await sut.SaveNewRecordAsync(age, name);
        key.Should().NotBeNull();
        key.Value.Should().NotBeNullOrWhiteSpace();

        var actual = await sut.GetRecordAsync(key);
        var expected = new AdvancedEntity
        {
            Id = new(key.Value),
            Age = age,
            Name = name,
            CreatedDate = DateTimeService.UtcNow
        };
        actual.Should().BeEquivalentTo(expected);
    }
}