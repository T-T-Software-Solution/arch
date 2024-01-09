using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data.Models;
using TTSS.Tests;

namespace TTSS.Core.Data;

public abstract class InMemoryRepositoryCommonTestCases : IoCTestBase
{
    private IRepository<BasicDbModel> BasicSut => ServiceProvider.GetRequiredService<IRepository<BasicDbModel>>();
    private IRepository<CustomPrimaryKeyDbModel, int> CustomSut => ServiceProvider.GetRequiredService<IRepository<CustomPrimaryKeyDbModel, int>>();

    [Fact]
    public void Resolve_InMemoryRepository_ShouldBeRegistered()
    {
        BasicSut.Should().NotBeNull();
        CustomSut.Should().NotBeNull();
    }

    #region Insert

    [Theory]
    [InlineData("1", "One")]
    [InlineData("2", "")]
    [InlineData("3", null)]
    public async Task Insert_BasicDbModel_ShouldSaveDataAsItBe(string id, string name)
    {
        var sut = BasicSut;
        var data = new BasicDbModel { Id = id, Name = name };
        await sut.InsertAsync(data);
        var actual = await sut.GetByIdAsync(id);
        actual.Should().Be(data);
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(name);
        sut.Get().Should().HaveCount(1);
    }

    [Theory]
    [InlineData(1, "One")]
    [InlineData(2, "")]
    [InlineData(3, null)]
    public async Task Insert_CustomDbModel_ShouldSaveDataAsItBe(int id, string name)
    {
        var sut = CustomSut;
        var data = new CustomPrimaryKeyDbModel { Id = id, Name = name };
        await sut.InsertAsync(data);
        var actual = await sut.GetByIdAsync(id);
        actual.Should().Be(data);
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(name);
        sut.Get().Should().HaveCount(1);
    }

    #endregion

    #region Update

    [Theory]
    [InlineData("1", "One", "1")]
    [InlineData("2", "", "empty")]
    [InlineData("3", null, "null")]
    public async Task Update_BasicDbModel_ShouldChangeTheRightObject(string id, string name, string newName)
    {
        var sut = BasicSut;
        await sut.InsertAsync(new BasicDbModel { Id = id, Name = name });
        var operationResult = await sut.UpdateAsync(id, new BasicDbModel { Id = id, Name = newName });
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(id);
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(newName);

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task Update_BasicDbModel_WithMismatchId_Then_NothingChanged()
    {
        const string Id = "1";
        var sut = BasicSut;
        await sut.InsertAsync(new BasicDbModel { Id = Id, Name = "One" });

        const string TargetUpdateId = "999";
        var operationResult = await sut.UpdateAsync(TargetUpdateId, new BasicDbModel
        {
            Id = Id,
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

    [Theory]
    [InlineData(1, "One", "1")]
    [InlineData(2, "", "empty")]
    [InlineData(3, null, "null")]
    public async Task Update_CustomDbModel_ShouldChangeTheRightObject(int id, string name, string newName)
    {
        var sut = CustomSut;
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = id, Name = name });
        var operationResult = await sut.UpdateAsync(id, new CustomPrimaryKeyDbModel { Id = id, Name = newName });
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(id);
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(newName);

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task Update_CustomDbModel_WithMismatchId_Then_NothingChanged()
    {
        const int Id = 1;
        var sut = CustomSut;
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = Id, Name = "One" });

        const int TargetUpdateId = 999;
        var operationResult = await sut.UpdateAsync(TargetUpdateId, new CustomPrimaryKeyDbModel
        {
            Id = Id,
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
    public async Task Delete_BasicDbModel_TheSelectedItem_MustDismiss()
    {
        const string Id = "1";
        var data = new BasicDbModel { Id = Id, Name = "One" };
        var sut = BasicSut;
        await sut.InsertAsync(data);

        var operationResult = await sut.DeleteAsync(Id);
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(Id);
        actual.Should().BeNull();

        sut.Get().Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_BasicDbModel_WithMismatchId_Then_NothingChanged()
    {
        const string Id = "1";
        var sut = BasicSut;
        await sut.InsertAsync(new BasicDbModel { Id = Id, Name = "One" });

        const string TargetUpdateId = "999";
        var operationResult = await sut.DeleteAsync(TargetUpdateId);
        operationResult.Should().BeFalse();

        var actual = await sut.GetByIdAsync(Id);
        actual.Id.Should().Be(Id);
        actual.Name.Should().Be("One");

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteMany_BasicDbModel_TheMatchedItems_MustDismiss()
    {
        var sut = BasicSut;
        await sut.InsertAsync(new BasicDbModel { Id = "1", Name = "One" });
        await sut.InsertAsync(new BasicDbModel { Id = "2", Name = "Two" });
        await sut.InsertAsync(new BasicDbModel { Id = "3", Name = "Three" });

        var target = new[] { "1", "2" };
        var operationResult = await sut.DeleteManyAsync(it => target.Contains(it.Id));
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync("3");
        actual.Id.Should().Be("3");
        actual.Name.Should().Be("Three");

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteMany_BasicDbModel_WithMismatchId_Then_NothingChanged()
    {
        var sut = BasicSut;
        await sut.InsertAsync(new BasicDbModel { Id = "1", Name = "One" });
        await sut.InsertAsync(new BasicDbModel { Id = "2", Name = "Two" });
        await sut.InsertAsync(new BasicDbModel { Id = "3", Name = "Three" });

        var target = new[] { "100" };
        var operationResult = await sut.DeleteManyAsync(it => target.Contains(it.Id));
        operationResult.Should().BeFalse();

        sut.Get().Should().HaveCount(3);
    }

    [Fact]
    public async Task Delete_CustomDbModel_TheSelectedItem_MustDismiss()
    {
        const int Id = 1;
        var data = new CustomPrimaryKeyDbModel { Id = Id, Name = "One" };
        var sut = CustomSut;
        await sut.InsertAsync(data);

        var operationResult = await sut.DeleteAsync(Id);
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(Id);
        actual.Should().BeNull();

        sut.Get().Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_CustomDbModel_WithMismatchId_Then_NothingChanged()
    {
        const int Id = 1;
        var sut = CustomSut;
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = Id, Name = "One" });

        const int TargetUpdateId = 999;
        var operationResult = await sut.DeleteAsync(TargetUpdateId);
        operationResult.Should().BeFalse();

        var actual = await sut.GetByIdAsync(Id);
        actual.Id.Should().Be(Id);
        actual.Name.Should().Be("One");

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteMany_CustomDbModel_TheMatchedItems_MustDismiss()
    {
        var sut = CustomSut;
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 1, Name = "One" });
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 2, Name = "Two" });
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 3, Name = "Three" });

        var operationResult = await sut.DeleteManyAsync(it => it.Id < 3);
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(3);
        actual.Id.Should().Be(3);
        actual.Name.Should().Be("Three");

        sut.Get().Should().HaveCount(1);
    }

    [Fact]
    public async Task DeleteMany_CustomDbModel_WithMismatchId_Then_NothingChanged()
    {
        var sut = CustomSut;
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 1, Name = "One" });
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 2, Name = "Two" });
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 3, Name = "Three" });

        var operationResult = await sut.DeleteManyAsync(it => it.Id > 100);
        operationResult.Should().BeFalse();

        sut.Get().Should().HaveCount(3);
    }

    #endregion

    #region Upsert

    [Fact]
    public async Task Upsert_BasicDbModel_ShouldWorkForBothExistAndNew()
    {
        var sut = BasicSut;
        await sut.InsertAsync(new BasicDbModel { Id = "1", Name = "One" });

        await sut.UpsertAsync("1", new BasicDbModel
        {
            Id = "1",
            Name = "1",
        });
        await sut.UpsertAsync("2", new BasicDbModel
        {
            Id = "2",
            Name = "Two",
        });

        var d1 = await sut.GetByIdAsync("1");
        d1.Id.Should().Be("1");
        d1.Name.Should().Be("1");

        var d2 = await sut.GetByIdAsync("2");
        d2.Id.Should().Be("2");
        d2.Name.Should().Be("Two");

        sut.Get().Should().HaveCount(2);
    }

    [Fact]
    public async Task Upsert_CustomDbModel_ShouldWorkForBothExistAndNew()
    {
        var sut = CustomSut;
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 1, Name = "One" });

        await sut.UpsertAsync(1, new CustomPrimaryKeyDbModel
        {
            Id = 1,
            Name = "1",
        });
        await sut.UpsertAsync(2, new CustomPrimaryKeyDbModel
        {
            Id = 2,
            Name = "Two",
        });

        var d1 = await sut.GetByIdAsync(1);
        d1.Id.Should().Be(1);
        d1.Name.Should().Be("1");

        var d2 = await sut.GetByIdAsync(2);
        d2.Id.Should().Be(2);
        d2.Name.Should().Be("Two");

        sut.Get().Should().HaveCount(2);
    }

    #endregion

    #region Query

    [Fact]
    public async Task Query_BasicDbModel_UsingCondition_MustGetAllMatchedItems()
    {
        var sut = BasicSut;
        await sut.InsertAsync(new BasicDbModel { Id = "1", Name = "One" });
        await sut.InsertAsync(new BasicDbModel { Id = "2", Name = "Two" });
        await sut.InsertAsync(new BasicDbModel { Id = "3", Name = "Three" });

        var target = new[] { "2", "3" };
        var actual = sut.Get(it => target.Contains(it.Id));
        actual.Should().HaveCount(2);

        actual.Select(it => it.Name).Should().BeEquivalentTo("Two", "Three");
    }

    [Fact]
    public async Task Query_BasicDbModel_UsingQuery_MustGetAllMatchedItems()
    {
        var sut = BasicSut;
        await sut.InsertAsync(new BasicDbModel { Id = "1", Name = "One" });
        await sut.InsertAsync(new BasicDbModel { Id = "2", Name = "Two" });
        await sut.InsertAsync(new BasicDbModel { Id = "3", Name = "Three" });

        var qry = from it in sut.Query()
                  where it.Name.StartsWith("T")
                  select it.Name;

        qry.Should().BeEquivalentTo("Two", "Three");
    }

    [Fact]
    public async Task Query_CustomDbModel_UsingCondition_MustGetAllMatchedItems()
    {
        var sut = CustomSut;
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 1, Name = "One" });
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 2, Name = "Two" });
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 3, Name = "Three" });

        var actual = sut.Get(it => it.Id >= 2);
        actual.Should().HaveCount(2);

        actual.Select(it => it.Name).Should().BeEquivalentTo("Two", "Three");
    }

    [Fact]
    public async Task Query_CustomDbModel_UsingQuery_MustGetAllMatchedItems()
    {
        var sut = CustomSut;
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 1, Name = "One" });
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 2, Name = "Two" });
        await sut.InsertAsync(new CustomPrimaryKeyDbModel { Id = 3, Name = "Three" });

        var qry = from it in sut.Query()
                  where it.Name.StartsWith("T")
                  select it.Name;

        qry.Should().BeEquivalentTo("Two", "Three");
    }

    #endregion

    #region No contents

    [Fact]
    public async Task GetPaging_WhenNoData_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, 0, 0, 0, false, false, 0, 0);

    [Fact]
    public async Task GetPaging_WhenNoData_WithTheSecondPage_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, 1, 0, 0, true, false, 0, 0);

    [Fact]
    public async Task GetPaging_WhenNoData_WithTheThirdPage_ThenTheSystemShouldNotError()
        => await ValidatePagingResult(0, 5, 2, 0, 0, true, false, 0, 0);

    #endregion

    #region Get 1st page

    [Fact]
    public async Task GetPaging_WhenDataAreLessThanPageSize()
        => await ValidatePagingResult(3, 5, 0, 0, 0, false, false, 1, 3);

    [Fact]
    public async Task GetPaging_WhenDataAreEqualWithPageSize()
        => await ValidatePagingResult(5, 5, 0, 0, 0, false, false, 1, 5);

    [Fact]
    public async Task GetPaging_WhenDataAreMoreThanPageSize()
        => await ValidatePagingResult(7, 5, 0, 0, 1, false, true, 2, 5);

    #endregion

    #region Get 2nd page

    [Fact]
    public async Task GetPaging_WithTheSecondPage_ThatHasLessThanPageSize()
       => await ValidatePagingResult(7, 5, 1, 0, 1, true, false, 2, 2);

    [Fact]
    public async Task GetPaging_WithTheSecondPage_ThatHasEqualWithPageSize()
        => await ValidatePagingResult(10, 5, 1, 0, 1, true, false, 2, 5);

    [Fact]
    public async Task GetPaging_WithTheSecondPage_ThatHasMoreThanPageSize()
        => await ValidatePagingResult(13, 5, 1, 0, 2, true, true, 3, 5);

    #endregion

    #region Get 3rd page

    [Fact]
    public async Task GetPaging_WithTheThirdPage_ThatHasLessThanPageSize()
        => await ValidatePagingResult(13, 5, 2, 1, 2, true, false, 3, 3);

    [Fact]
    public async Task GetPaging_WithTheThirdPage_ThatHasEqualWithPageSize()
        => await ValidatePagingResult(15, 5, 2, 1, 2, true, false, 3, 5);

    [Fact]
    public async Task GetPaging_WithTheThirdPage_ThatHasMoreThanPageSize()
        => await ValidatePagingResult(30, 5, 2, 1, 3, true, true, 6, 5);

    #endregion

    private async Task ValidatePagingResult(int contents, int pageSize, int getPageNo,
        int expectedPrevPage, int expectedNextPage,
        bool expectedHasPrevPage, bool expectedHasNextPage,
        int expectedPageCount, int expectedDataElements)
    {
        var sut = CustomSut;
        var records = Enumerable.Range(1, contents)
            .Select(it => new CustomPrimaryKeyDbModel { Id = it, Name = it.ToString() });

        foreach (var item in records)
        {
            await sut.InsertAsync(item);
        }

        var paging = sut.Get().ToPaging(totalCount: true, pageSize);
        var pagingResult = paging.GetPage(getPageNo);
        pagingResult.CurrentPage.Should().Be(getPageNo);
        pagingResult.PreviousPage.Should().Be(expectedPrevPage);
        pagingResult.NextPage.Should().Be(expectedNextPage);
        pagingResult.HasPreviousPage.Should().Be(expectedHasPrevPage);
        pagingResult.HasNextPage.Should().Be(expectedHasNextPage);
        pagingResult.TotalCount.Should().Be(contents);
        pagingResult.PageCount.Should().Be(expectedPageCount);
        (await pagingResult.GetDataAsync()).Should().HaveCount(expectedDataElements);

        var pagingData = await pagingResult.ToPagingDataAsync();
        pagingData.CurrentPage.Should().Be(getPageNo);
        pagingData.PreviousPage.Should().Be(expectedPrevPage);
        pagingData.NextPage.Should().Be(expectedNextPage);
        pagingData.HasPreviousPage.Should().Be(expectedHasPrevPage);
        pagingData.HasNextPage.Should().Be(expectedHasNextPage);
        pagingData.TotalCount.Should().Be(contents);
        pagingData.PageCount.Should().Be(expectedPageCount);
        pagingData.Result.Should().HaveCount(expectedDataElements);
        pagingData.Result.Should().BeEquivalentTo((await pagingResult.GetDataAsync()).ToList());
    }
}