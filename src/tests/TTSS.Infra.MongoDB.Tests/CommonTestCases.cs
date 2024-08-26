using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using TTSS.Core.Data;
using TTSS.Infra.Data.MongoDB.Documents;
using TTSS.Tests;

namespace TTSS.Infra.Data.MongoDB;

public abstract class CommonTestCases : IoCTestBase, IDisposable
{
    private volatile static int RunningInstances;
    private static readonly Lazy<MongoDbRunner> DbRunner = new(() => MongoDbRunner.Start());

    public static string ConnectionString => DbRunner.Value.ConnectionString;

    public CommonTestCases()
    {
        RunningInstances++;
    }

    public virtual void Dispose()
    {
        if (--RunningInstances <= 0)
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                if (RunningInstances <= 0) DbRunner.Value.Dispose();
            });
        }
        GC.SuppressFinalize(this);
    }

    #region Insert

    [Fact]
    public async Task Insert_AllDataValid_TheDataCanBeSaveAndRead()
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();

        var data = Fixture.Create<Person>();
        await sut.InsertAsync(data);

        (await sut.GetByIdAsync(data.Id)).Should().BeEquivalentTo(data);
        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task Insert_WithNullId_ThenItMustNotThrowAnException()
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = null, Name = Guid.NewGuid().ToString() });
    }

    [Fact]
    public async Task Insert_WithDuplicateKey_ThenSystemMustThrowAnException()
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        var data = Fixture.Create<Person>();
        var insertion = async () =>
        {
            await sut.InsertAsync(data);
            await sut.InsertAsync(data);
        };
        await insertion.Should()
            .ThrowAsync<MongoWriteException>()
            .Where(it => it.Message.Contains("DuplicateKey"));
    }

    #endregion

    #region Update

    [Theory]
    [InlineAutoData(1, "One", "1")]
    [InlineAutoData(2, "", "empty")]
    [InlineAutoData(3, null, "null")]
    public async Task Update_ShouldChangeTheRightObject(string id, string name, string newName)
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = id, Name = name });

        var operationResult = await sut.UpdateAsync(new Person { Id = id, Name = newName });
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(id);
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(newName);

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task Update_WithMismatchId_Then_NothingChanged()
    {
        const string Id = "1";
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = Id, Name = "One" });

        const string TargetUpdateId = "999";
        var operationResult = await sut.UpdateAsync(new Person
        {
            Id = TargetUpdateId,
            Name = "Anything"
        });
        operationResult.Should().BeFalse();

        var actual = await sut.GetByIdAsync(Id);
        actual.Id.Should().Be(Id);
        actual.Name.Should().Be("One");

        var notfound = await sut.GetByIdAsync(TargetUpdateId);
        notfound.Should().BeNull();

        sut.Get().Should().HaveCount(1);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_TheSelectedItem_MustDismiss()
    {
        const string Id = "1";
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = Id, Name = "One" });

        var operationResult = await sut.DeleteAsync(Id);
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(Id);
        actual.Should().BeNull();

        sut.Get().Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_WithMismatchId_Then_NothingChanged()
    {
        const string Id = "1";
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = Id, Name = "One" });

        const string TargetUpdateId = "999";
        var operationResult = await sut.DeleteAsync(TargetUpdateId);
        operationResult.Should().BeFalse();

        var actual = await sut.GetByIdAsync(Id);
        actual.Id.Should().Be(Id);
        actual.Name.Should().Be("One");

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteMany_TheMatchedItems_MustDismiss()
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = "1", Name = "One" });
        await sut.InsertAsync(new Person { Id = "2", Name = "Two" });
        await sut.InsertAsync(new Person { Id = "3", Name = "Three" });

        var operationResult = await sut.DeleteManyAsync(it => it.Name.ToLower().Contains('o'));
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync("3");
        actual.Id.Should().Be("3");
        actual.Name.Should().Be("Three");

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteMany_WithMismatchId_Then_NothingChanged()
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = "1", Name = "One" });
        await sut.InsertAsync(new Person { Id = "2", Name = "Two" });
        await sut.InsertAsync(new Person { Id = "3", Name = "Three" });

        var operationResult = await sut.DeleteManyAsync(it => it.Name.Contains("NONE"));
        operationResult.Should().BeFalse();

        sut.Get().Should().HaveCount(3);
    }

    #endregion

    #region Upsert

    [Fact]
    public async Task Upsert_WithNotExistDocument_ShouldCreateNewDocument()
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        var operationResult1 = await sut.UpsertAsync("1", new Person
        {
            Id = "1",
            Name = "1",
        });
        operationResult1.Should().BeTrue();
        var d1 = await sut.GetByIdAsync("1");
        d1.Id.Should().Be("1");
        d1.Name.Should().Be("1");
    }

    [Fact]
    public async Task Upsert_WithExistDocument_ShouldUpdateTheDocument()
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = "1", Name = "One" });

        var operationResult1 = await sut.UpsertAsync("1", new Person
        {
            Id = "1",
            Name = "1",
        });
        operationResult1.Should().BeTrue();
        var d1 = await sut.GetByIdAsync("1");
        d1.Id.Should().Be("1");
        d1.Name.Should().Be("1");
    }

    [Fact]
    public async Task Upsert_ShouldWorkForBothExistAndNew()
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(new Person { Id = "1", Name = "One" });

        var operationResult1 = await sut.UpsertAsync("1", new Person
        {
            Id = "1",
            Name = "1",
        });
        operationResult1.Should().BeTrue();

        var operationResult2 = await sut.UpsertAsync("2", new Person
        {
            Id = "2",
            Name = "Two",
        });
        operationResult2.Should().BeTrue();

        var d1 = await sut.GetByIdAsync("1");
        d1.Id.Should().Be("1");
        d1.Name.Should().Be("1");

        var d2 = await sut.GetByIdAsync("2");
        d2.Id.Should().Be("2");
        d2.Name.Should().Be("Two");

        sut.Get().Should().HaveCount(2);
    }

    #endregion

    #region InsertBulk

    [Fact]
    public async Task InsertBulk_ShouldWorkAsExpected()
    {
        const int TotalElementCount = 100;
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        var data = Fixture.CreateMany<Person>(TotalElementCount);
        await sut.InsertBulkAsync(data);
        var actual = sut.Get();
        actual.Should().HaveCount(TotalElementCount);
        actual.Should().BeEquivalentTo(data);
        sut.Get().Should().HaveCount(TotalElementCount);
    }

    #endregion

    #region IoC

    [Fact]
    public void MongoRepository_CanResolveAllRepositoryTypes()
    {
        ServiceProvider.GetService<IRepository<Person>>().Should().NotBeNull();
        ServiceProvider.GetService<IMongoDbRepository<Person>>().Should().NotBeNull();
        ServiceProvider.GetService<IRepository<Person, string>>().Should().NotBeNull();
        ServiceProvider.GetService<IMongoDbRepository<Person, string>>().Should().NotBeNull();
    }

    [Fact]
    public async Task MongoRepository_ShouldWorkProperlyOnIoC_ViaIRepository()
    {
        var person = Fixture.Create<Person>();
        var sut = ServiceProvider.GetService<IRepository<Person, string>>();
        await sut.InsertAsync(person);
        var actual = await sut.GetByIdAsync(person.Id);
        actual.Should().BeEquivalentTo(person);
        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task MongoRepository_ShouldWorkProperlyOnIoC_ViaIMongoRepository()
    {
        var person = Fixture.Create<Person>();
        var sut = ServiceProvider.GetService<IMongoDbRepository<Person, string>>();
        await sut.InsertAsync(person);
        var actual = await sut.GetByIdAsync(person.Id);
        actual.Should().BeEquivalentTo(person);
        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task MongoRepository_ShouldWorkProperlyOnIoC_ViaSimplestIRepository()
    {
        var person = Fixture.Create<Person>();
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(person);
        var actual = await sut.GetByIdAsync(person.Id);
        actual.Should().BeEquivalentTo(person);
        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task MongoRepository_ShouldWorkProperlyOnIoC_ViaSimplestIMongoRepository()
    {
        var person = Fixture.Create<Person>();
        var sut = ServiceProvider.GetService<IMongoDbRepository<Person>>();
        await sut.InsertAsync(person);
        var actual = await sut.GetByIdAsync(person.Id);
        actual.Should().BeEquivalentTo(person);
        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task Insert_FromAnyRepositoryType_MustExactTheSameResul()
    {
        var sut1 = ServiceProvider.GetService<IRepository<Person>>();
        var sut2 = ServiceProvider.GetService<IMongoDbRepository<Person>>();
        var sut3 = ServiceProvider.GetService<IRepository<Person, string>>();
        var sut4 = ServiceProvider.GetService<IMongoDbRepository<Person, string>>();

        var data = Fixture.Create<Person>();
        await sut1.InsertAsync(data);

        (await sut1.GetByIdAsync(data.Id)).Should().BeEquivalentTo(data);
        (await sut2.GetByIdAsync(data.Id)).Should().BeEquivalentTo(data);
        (await sut3.GetByIdAsync(data.Id)).Should().BeEquivalentTo(data);
        (await sut4.GetByIdAsync(data.Id)).Should().BeEquivalentTo(data);

        sut1.Get().Should().HaveCount(1);
        sut2.Get().Should().HaveCount(1);
        sut3.Get().Should().HaveCount(1);
        sut4.Get().Should().HaveCount(1);
    }

    #endregion

    #region Paging

    #region No contents

    [Fact]
    public async Task GetPaging_WhenNoData_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, 1, 1, 1, false, false, 1, 0);

    [Fact]
    public async Task GetPaging_WhenNoData_WithTheSecondPage_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, 2, 1, 1, true, false, 1, 0);

    [Fact]
    public async Task GetPaging_WhenNoData_WithTheThirdPage_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, 3, 1, 1, true, false, 1, 0);

    [Fact]
    public async Task GetPaging_WhenNoData_WithTheVeryFarPage_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, 9999, 1, 1, true, false, 1, 0);

    [Fact]
    public async Task GetPaging_WhenNoData_WithZeroPage_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, 0, 1, 1, false, false, 1, 0);

    [Fact]
    public async Task GetPaging_WhenNoData_WithNegativePage_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, -9999, 1, 1, false, false, 1, 0);

    #endregion

    #region Get 1st page

    [Fact]
    public async Task GetPaging_WhenDataAreLessThanPageSize()
        => await ValidatePagingResult(3, 5, 1, 1, 1, false, false, 1, 3);

    [Fact]
    public async Task GetPaging_WhenDataAreEqualWithPageSize()
        => await ValidatePagingResult(5, 5, 1, 1, 1, false, false, 1, 5);

    [Fact]
    public async Task GetPaging_WhenDataAreMoreThanPageSize()
        => await ValidatePagingResult(7, 5, 1, 1, 2, false, true, 2, 5);

    #endregion

    #region Get 2nd page

    [Fact]
    public async Task GetPaging_WithTheSecondPage_ThatHasLessThanPageSize()
       => await ValidatePagingResult(7, 5, 2, 1, 2, true, false, 2, 2);

    [Fact]
    public async Task GetPaging_WithTheSecondPage_ThatHasEqualWithPageSize()
        => await ValidatePagingResult(10, 5, 2, 1, 2, true, false, 2, 5);

    [Fact]
    public async Task GetPaging_WithTheSecondPage_ThatHasMoreThanPageSize()
        => await ValidatePagingResult(13, 5, 2, 1, 3, true, true, 3, 5);

    #endregion

    #region Get 3rd page

    [Fact]
    public async Task GetPaging_WithTheThirdPage_ThatHasLessThanPageSize()
        => await ValidatePagingResult(13, 5, 3, 2, 3, true, false, 3, 3);

    [Fact]
    public async Task GetPaging_WithTheThirdPage_ThatHasEqualWithPageSize()
        => await ValidatePagingResult(15, 5, 3, 2, 3, true, false, 3, 5);

    [Fact]
    public async Task GetPaging_WithTheThirdPage_ThatHasMoreThanPageSize()
        => await ValidatePagingResult(30, 5, 3, 2, 4, true, true, 6, 5);

    #endregion

    private async Task ValidatePagingResult(int contents, int pageSize, int getPageNo,
        int expectedPrevPage, int expectedNextPage,
        bool expectedHasPrevPage, bool expectedHasNextPage,
        int expectedPageCount, int expectedDataElements)
    {
        var sut = ServiceProvider.GetService<IRepository<Person>>();
        var records = Enumerable
            .Range(1, contents)
            .Select(it => Fixture.Create<Person>());
        await sut.InsertBulkAsync(records);

        var repository = sut.Get().ToPaging(totalCount: true, pageSize);
        var pagingSet = repository.GetPage(getPageNo);
        pagingSet.CurrentPage.Should().Be(getPageNo);
        pagingSet.PreviousPage.Should().Be(expectedPrevPage);
        pagingSet.NextPage.Should().Be(expectedNextPage);
        pagingSet.HasPreviousPage.Should().Be(expectedHasPrevPage);
        pagingSet.HasNextPage.Should().Be(expectedHasNextPage);
        pagingSet.TotalCount.Should().Be(contents);
        pagingSet.PageCount.Should().Be(expectedPageCount);
        (await pagingSet.GetDataAsync()).Should().HaveCount(expectedDataElements);

        var paging = await pagingSet.ExecuteAsync();
        paging.CurrentPage.Should().Be(getPageNo);
        paging.PreviousPage.Should().Be(expectedPrevPage);
        paging.NextPage.Should().Be(expectedNextPage);
        paging.HasPreviousPage.Should().Be(expectedHasPrevPage);
        paging.HasNextPage.Should().Be(expectedHasNextPage);
        paging.TotalCount.Should().Be(contents);
        paging.PageCount.Should().Be(expectedPageCount);
        paging.Contents.Should().HaveCount(expectedDataElements);
        paging.Contents.Should().BeEquivalentTo((await pagingSet.GetDataAsync()).ToList());
    }

    #endregion
}