﻿using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.DbContexte;
using TTSS.Infra.Data.Sql.DbModels;
using TTSS.Infra.Data.Sql.Models;
using TTSS.Tests;

namespace TTSS.Infra.Data.Sql;

public abstract class CommonTestCases : IoCTestBase, IDisposable
{
    public abstract void Dispose();

    #region Resolve

    [Fact]
    public void Resolve_IRepository_ShouldBeSuccessful()
    {
        ServiceProvider.GetRequiredService<IRepository<Apple>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<IRepository<Banana>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<IRepository<Student>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<IRepository<Teacher>>().Should().NotBeNull();
    }

    [Fact]
    public void Resolve_IRepository_WithSpecificKeyType_ShouldBeSuccessful()
    {
        ServiceProvider.GetRequiredService<IRepository<Apple, string>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<IRepository<Banana, string>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<IRepository<Student, string>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<IRepository<Teacher, string>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<IRepository<Principal, int>>().Should().NotBeNull();
    }

    [Fact]
    public void Resolve_ISqlRepository_ShouldBeSuccessful()
    {
        ServiceProvider.GetRequiredService<ISqlRepository<Apple>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<ISqlRepository<Banana>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<ISqlRepository<Student>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<ISqlRepository<Teacher>>().Should().NotBeNull();
    }

    [Fact]
    public void Resolve_ISqlRepository_WithSpecificKeyType_ShouldBeSuccessful()
    {
        ServiceProvider.GetRequiredService<ISqlRepository<Apple, string>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<ISqlRepository<Banana, string>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<ISqlRepository<Student, string>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<ISqlRepository<Teacher, string>>().Should().NotBeNull();
        ServiceProvider.GetRequiredService<ISqlRepository<Principal, int>>().Should().NotBeNull();
    }

    [Fact]
    public void Resolve_ISqlRepositorySpecific_ShouldBeSuccessful()
    {
        ServiceProvider.GetService<ISqlRepositorySpecific<Apple>>().Should().BeNull();
        ServiceProvider.GetService<ISqlRepositorySpecific<Banana>>().Should().BeNull();
        ServiceProvider.GetService<ISqlRepositorySpecific<Student>>().Should().BeNull();
        ServiceProvider.GetService<ISqlRepositorySpecific<Teacher>>().Should().BeNull();
    }

    [Fact]
    public void Resolve_SqlRepositorySpecific_ShouldBeSuccessful()
    {
        ServiceProvider.GetService<SqlRepository<Apple>>().Should().BeNull();
        ServiceProvider.GetService<SqlRepository<Banana>>().Should().BeNull();
        ServiceProvider.GetService<SqlRepository<Student>>().Should().BeNull();
        ServiceProvider.GetService<SqlRepository<Teacher>>().Should().BeNull();
    }

    [Fact]
    public void Resolve_SqlRepositorySpecific_WithSpecificKeyType_ShouldBeSuccessful()
    {
        ServiceProvider.GetService<SqlRepository<Apple, string>>().Should().BeNull();
        ServiceProvider.GetService<SqlRepository<Banana, string>>().Should().BeNull();
        ServiceProvider.GetService<SqlRepository<Student, string>>().Should().BeNull();
        ServiceProvider.GetService<SqlRepository<Teacher, string>>().Should().BeNull();
        ServiceProvider.GetService<SqlRepository<Principal, int>>().Should().BeNull();
    }

    #endregion

    #region Insert

    [Fact(DisplayName = "เพิ่มข้อมูลรายการเดียว ระบบสามารถบันทึกข้อมูลได้ถูกต้อง")]
    public async Task Insert_AllDataValid_ShouldInsertNewRecord()
    {
        var apple = Fixture.Create<Apple>();
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();

        await sut.InsertAsync(apple);

        var actual = await sut.GetByIdAsync(apple.Id);
        actual.Should().BeEquivalentTo(apple);
        actual.Id.Should().Be(apple.Id);
        actual.Name.Should().Be(apple.Name);

        sut.Get().Should().BeEquivalentTo(new[] { apple });
    }

    [Fact(DisplayName = "เพิ่มข้อมูลที่เป็นคลาสเครือญาติกันอย่างละรายการ ระบบสามารถบันทึกข้อมูลโดยแยกตารางได้ถูกต้อง")]
    public async Task Insert_SingleRecored_WithoutDiscriminator_ShouldSaveDataSeparatly()
    {
        var appleRepo = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        await appleRepo.InsertAsync(Fixture.Create<Apple>());

        var bananaRepo = ServiceProvider.GetRequiredService<IRepository<Banana>>();
        await bananaRepo.InsertAsync(Fixture.Create<Banana>());

        appleRepo.Get().Should().HaveCount(1);
        bananaRepo.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "เพิ่มข้อมูลที่เป็นคลาสเครือญาติกันหลายรายการ ระบบสามารถบันทึกข้อมูลโดยแยกตารางได้ถูกต้อง")]
    public async Task Insert_MultipleRecords_WithoutDiscriminator_ShouldSaveDataSeparatly()
    {
        var appleRepo = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        var appleList = new[]
        {
                Fixture.Create<Apple>(),
                Fixture.Create<Apple>(),
                Fixture.Create<Apple>(),
            };
        foreach (var item in appleList)
        {
            await appleRepo.InsertAsync(item);
        }

        var bananaRepo = ServiceProvider.GetRequiredService<IRepository<Banana>>();
        var bananaList = new[]
        {
                Fixture.Create<Banana>(),
                Fixture.Create<Banana>(),
                Fixture.Create<Banana>(),
                Fixture.Create<Banana>(),
                Fixture.Create<Banana>(),
            };
        foreach (var item in bananaList)
        {
            await bananaRepo.InsertAsync(item);
        }

        appleRepo.Get().Should().BeEquivalentTo(appleList);
        bananaRepo.Get().Should().BeEquivalentTo(bananaList);
    }

    [Fact(DisplayName = "เพิ่มข้อมูลที่มีความสัมพันธ์กับตารางอื่น ระบบต้องบันทึกข้อมูลใหม่และข้อมูลความสัมพันธ์ที่เกี่ยวข้องทุกตัวด้วย")]
    public async Task Insert_ObjectWithNavigationProperty_ThenSystemMustInsertTheNewRecordAndTheNavigationProperty()
    {
        var teacher = new Teacher
        {
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            Salary = 500,
        };
        var student1 = new Student(teacher)
        {
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
        };
        var student2 = new Student(teacher)
        {
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
        };
        var studentRepo = ServiceProvider.GetRequiredService<IRepository<Student>>();
        var teacherRepo = ServiceProvider.GetRequiredService<IRepository<Teacher>>();

        await studentRepo.InsertAsync(student1);
        await studentRepo.InsertAsync(student2);

        studentRepo.Get().Should().BeEquivalentTo(new[] { student1, student2 });
        teacherRepo.Get().Should().BeEquivalentTo(new[] { teacher });

        (await studentRepo.GetByIdAsync(student1.Id)).Should().BeEquivalentTo(student1);
        (await studentRepo.GetByIdAsync(student2.Id)).Should().BeEquivalentTo(student2);
        (await teacherRepo.GetByIdAsync(teacher.Id)).Should().BeEquivalentTo(teacher);

        teacher.Students.Should().BeEquivalentTo(new[] { student1, student2 });
    }

    [Fact(DisplayName = "เพิ่มข้อมูลที่ Id เป็น null ระบบต้องไม่ทำการบันทึกข้อมูลและโยน exception ออกมา")]
    public async Task Insert_WithNullId_ThenItMustThrowAnException()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();

        var act = async () => await sut.InsertAsync(new Apple { Id = null, Name = Guid.NewGuid().ToString() });
        await act.Should().ThrowAsync<InvalidOperationException>("*primary key property '*' is null");
        sut.Get().Should().HaveCount(0);
    }

    [Fact(DisplayName = "เพิ่มข้อมูลที่ Id ซ้ำกัน ระบบต้องไม่ทำการบันทึกข้อมูลและโยน exception ออกมา")]
    public async Task Insert_WithDuplicateKey_ThenSystemMustThrowAnException()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        var firstRecord = Fixture.Create<Apple>();
        var secondRecord = Fixture.Create<Apple>();
        secondRecord.Id = firstRecord.Id;

        Func<Task> action = async () =>
        {
            await sut.InsertAsync(firstRecord);
            await sut.InsertAsync(secondRecord);
        };
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("*same key*");
        sut.Get().Should().HaveCount(1).And.BeEquivalentTo(new[] { firstRecord });
    }

    #region Key is a number

    [Fact]
    public async Task Insert_ByUsingNumberAsAPrimaryKey_ThenItShouldInsertNewRecord()
    {
        var data = Fixture.Create<Principal>();
        var sut = ServiceProvider.GetRequiredService<IRepository<Principal, int>>();

        await sut.InsertAsync(data);

        var actual = await sut.GetByIdAsync(data.Id);
        actual.Should().BeEquivalentTo(data);
        actual.Id.Should().Be(data.Id);
        actual.Name.Should().Be(data.Name);

        sut.Get().Should().BeEquivalentTo(new[] { data });
    }

    #endregion

    #endregion

    #region Update

    [Theory(DisplayName = "อัพโดยใส่เดทข้อมูลถูกต้อง ระบบทำการอัพเดทข้อมูล")]
    [InlineAutoData(1, "One", "1")]
    [InlineAutoData(2, "Space", " ")]
    [InlineAutoData(3, "Empty", "")]
    public async Task Update_ShouldChangeTheRightObject(string id, string name, string newName)
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        var record = new Apple { Id = id, Name = name };
        await sut.InsertAsync(record);

        record.Name = newName;
        var operationResult = await sut.UpdateAsync(record);
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(id);
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(newName);

        sut.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "อัพเดทโดยส่งรหัสไม่ตรงกัน ระบบไม่ทำการอัพเดทข้อมูล")]
    public async Task Update_WithMismatchId_Then_NothingChanged()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        var id = Guid.NewGuid().ToString();
        var record = new Apple { Id = id, Name = "One" };
        await sut.InsertAsync(record);

        const string TargetUpdateId = "999";
        var newName = Guid.NewGuid().ToString();
        record.Name = newName;
        var operationResult = await sut.UpdateAsync(TargetUpdateId, record);
        operationResult.Should().BeFalse();

        var notfound = await sut.GetByIdAsync(TargetUpdateId);
        notfound.Should().BeNull();

        sut.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "อัพเดทโดยใช้ object ที่มาจากภายนอก ระบบไม่ทำการอัพเดทข้อมูลและโยน exception ออกมา")]
    public async Task Update_WithObjectFromExternal_ThenSystemMustThrowAnException()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        var id = Guid.NewGuid().ToString();
        var record = new Apple { Id = id, Name = Guid.NewGuid().ToString() };
        await sut.InsertAsync(record);

        Func<Task> action = async () =>
        {
            var update = new Apple { Id = id, Name = Guid.NewGuid().ToString() };
            await sut.UpdateAsync(update);
        };

        await action.Should().ThrowAsync<InvalidOperationException>();
        sut.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "อัพเดทโดยใส่ข้อมูลไม่ถูกต้อง ระบบไม่ทำการอัพเดทข้อมูลและโยน exception ออกมา")]
    public async Task Update_WithInvalidConstrain_ThenSystemMustThrowAnException()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        var record = Fixture.Create<Apple>();
        await sut.InsertAsync(record);

        Func<Task> action = async () =>
        {
            record.Name = null;
            await sut.UpdateAsync(record);
        };

        await action.Should().ThrowAsync<DbUpdateException>();
        sut.Get().Should().HaveCount(1);
    }

    #region Key is a number

    [Theory(DisplayName = "อัพโดยใส่เดทข้อมูลถูกต้อง ระบบทำการอัพเดทข้อมูล")]
    [InlineAutoData(1, "One", "1")]
    [InlineAutoData(2, "Space", " ")]
    [InlineAutoData(3, "Empty", "")]
    public async Task Update_ByRepositoryWithNoneStringKeyValue_ShouldChangeTheRightObject(int id, string name, string newName)
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Principal, int>>();
        var record = new Principal { Id = id, Name = name };
        await sut.InsertAsync(record);

        record.Name = newName;
        var operationResult = await sut.UpdateAsync(record);
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(id);
        actual.Id.Should().Be(id);
        actual.Name.Should().Be(newName);

        sut.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "อัพเดทโดยส่งรหัสไม่ตรงกัน ระบบไม่ทำการอัพเดทข้อมูล")]
    public async Task Update_ByRepositoryWithNoneStringKeyValue_WithMismatchId_Then_NothingChanged()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Principal, int>>();
        var id = 1;
        var record = new Principal { Id = id, Name = "One" };
        await sut.InsertAsync(record);

        const int TargetUpdateId = 999;
        var newName = Guid.NewGuid().ToString();
        record.Name = newName;
        var operationResult = await sut.UpdateAsync(TargetUpdateId, record);
        operationResult.Should().BeFalse();

        var notfound = await sut.GetByIdAsync(TargetUpdateId);
        notfound.Should().BeNull();

        sut.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "อัพเดทโดยใช้ object ที่มาจากภายนอก ระบบไม่ทำการอัพเดทข้อมูลและโยน exception ออกมา")]
    public async Task Update_ByRepositoryWithNoneStringKeyValue_WithObjectFromExternal_ThenSystemMustThrowAnException()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Principal, int>>();
        var id = 1;
        var record = new Principal { Id = id, Name = Guid.NewGuid().ToString() };
        await sut.InsertAsync(record);

        Func<Task> action = async () =>
        {
            var update = new Principal { Id = id, Name = Guid.NewGuid().ToString() };
            await sut.UpdateAsync(update);
        };

        await action.Should().ThrowAsync<InvalidOperationException>();
        sut.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "อัพเดทโดยใส่ข้อมูลไม่ถูกต้อง ระบบไม่ทำการอัพเดทข้อมูลและโยน exception ออกมา")]
    public async Task Update_ByRepositoryWithNoneStringKeyValue_WithInvalidConstrain_ThenSystemMustThrowAnException()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Principal, int>>();
        var record = Fixture.Create<Principal>();
        await sut.InsertAsync(record);

        Func<Task> action = async () =>
        {
            record.Name = null;
            await sut.UpdateAsync(record);
        };

        await action.Should().ThrowAsync<DbUpdateException>();
        sut.Get().Should().HaveCount(1);
    }

    #endregion

    #endregion

    #region Delete

    [Fact(DisplayName = "ลบข้อมูลถูกต้อง ระบบทำการลบรายการที่เลือก")]
    public async Task Delete_AllDataValid_ThenTheSelectedItemMustBeDeleted()
    {
        const string Id = "1";
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        await sut.InsertAsync(new Apple { Id = Id, Name = "One" });

        var operationResult = await sut.DeleteAsync(Id);
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(Id);
        actual.Should().BeNull();

        sut.Get().Should().BeEmpty();
    }

    [Fact(DisplayName = "ลบข้อมูลโดยระบุรหัสไม่ถูก ระบบไม่ทำการลบข้อมูล")]
    public async Task Delete_WithMismatchId_Then_NothingChanged()
    {
        const string Id = "1";
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        await sut.InsertAsync(new Apple { Id = Id, Name = "One" });

        const string TargetUpdateId = "999";
        var operationResult = await sut.DeleteAsync(TargetUpdateId);
        operationResult.Should().BeFalse();

        var actual = await sut.GetByIdAsync(Id);
        actual.Id.Should().Be(Id);
        actual.Name.Should().Be("One");

        sut.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "ลบข้อมูลหลายรายการ ระบบทำการลบข้อมูลที่ตรงกับเงื่อนไข")]
    public async Task DeleteMany_TheMatchedItems_MustBeDeleted()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        await sut.InsertAsync(new Apple { Id = "1", Name = "One" });
        await sut.InsertAsync(new Apple { Id = "2", Name = "Two" });
        await sut.InsertAsync(new Apple { Id = "3", Name = "Three" });

        var operationResult = await sut.DeleteManyAsync(it => it.Name.ToLower().Contains("o"));
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync("3");
        actual.Id.Should().Be("3");
        actual.Name.Should().Be("Three");

        sut.Get().Should().HaveCount(1);
    }

    [Fact(DisplayName = "ลบข้อมูลหลายรายการแต่ไม่มีข้อมูลที่ตรงกับเงื่อนไข ระบบไม่มีข้อมูลถูกลบออกจากระบบ")]
    public async Task DeleteMany_WithMismatchId_Then_NothingChanged()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        await sut.InsertAsync(new Apple { Id = "1", Name = "One" });
        await sut.InsertAsync(new Apple { Id = "2", Name = "Two" });
        await sut.InsertAsync(new Apple { Id = "3", Name = "Three" });

        var operationResult = await sut.DeleteManyAsync(it => it.Name.Contains("NONE"));
        operationResult.Should().BeFalse();

        sut.Get().Should().HaveCount(3);
    }

    #region Key is a number

    [Fact]
    public async Task Delete_TableThatHaveNumberAsPrimaryKey_AllDataValid_ThenTheSelectedItemMustBeDeleted()
    {
        const int Id = 1;
        var sut = ServiceProvider.GetRequiredService<IRepository<Principal, int>>();
        await sut.InsertAsync(new Principal { Id = Id, Name = "One" });

        var operationResult = await sut.DeleteAsync(Id);
        operationResult.Should().BeTrue();

        var actual = await sut.GetByIdAsync(Id);
        actual.Should().BeNull();

        sut.Get().Should().BeEmpty();
    }

    #endregion

    #endregion

    #region InsertBulk

    [Fact(DisplayName = "เพิ่มข้อมูลปริมาณเยอะๆ ระบบสามารถเพิ่มได้ถูกต้อง")]
    public async Task InsertBulk_ShouldWorkAsExpected()
    {
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        var qry = Enumerable.Range(1, 100)
            .Select(it => new Apple
            {
                Id = it.ToString(),
                Name = it.ToString(),
            });
        await sut.InsertBulkAsync(qry);
        var actual = sut.Get();
        actual.Should().HaveCount(100);
        actual.Should().BeEquivalentTo(qry);
    }

    #endregion

    #region Simplest Insert

    [Fact(DisplayName = "Repository ทั้งหมดต้องสามารถใช้งานแทนกันได้")]
    public async Task AllRepositoryTypes_CanBeUsedInterchangeably()
    {
        IRepository<Apple> simplestGenericRepo = ServiceProvider.GetRequiredService<IRepository<Apple>>();
        ISqlRepository<Apple> simplestSqlRepo = ServiceProvider.GetRequiredService<ISqlRepository<Apple>>();
        IRepository<Apple, string> genericRepo = ServiceProvider.GetRequiredService<IRepository<Apple, string>>();
        ISqlRepository<Apple, string> sqlRepo = ServiceProvider.GetRequiredService<ISqlRepository<Apple, string>>();

        var apples = Fixture.CreateMany<Apple>(4).ToArray();

        await simplestGenericRepo.InsertAsync(apples[0]);
        await simplestSqlRepo.InsertAsync(apples[1]);
        await genericRepo.InsertAsync(apples[2]);
        await sqlRepo.InsertAsync(apples[3]);

        simplestGenericRepo.Get().Should().BeEquivalentTo(apples);
        simplestSqlRepo.Get().Should().BeEquivalentTo(apples);
        genericRepo.Get().Should().BeEquivalentTo(apples);
        sqlRepo.Get().Should().BeEquivalentTo(apples);
    }

    #endregion

    #region Paging

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
        var sut = ServiceProvider.GetRequiredService<IRepository<Apple>>();

        var records = Enumerable.Range(1, contents)
            .Select(it => Fixture.Create<Apple>());

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

    #endregion

    #region Registrations

    [Theory]
    [InlineData(typeof(DbModel))]
    [InlineData(typeof(DbModelOfString))]
    [InlineData(typeof(DbModelOfInteger))]
    [InlineData(typeof(DbModelOfDouble))]
    [InlineData(typeof(DbModelOfLong))]
    [InlineData(typeof(DbModelOfGuid))]
    [InlineData(typeof(SqlDbModel))]
    [InlineData(typeof(ChildOfSqlDbModel))]
    [InlineData(typeof(ChildOfDbModel))]
    public void Create_SqlConnection_FromAnyKindsOfIDbModel_MustBeSuccessful(Type entityType)
    {
        var create = () => new SqlConnection(entityType, typeof(DbContextBase<>));
        create.Should().NotThrow();
    }

    [Fact]
    public void Create_SqlConnection_FromOutOfIDbModelFamily_MustThrowAnException()
    {
        var create = () => new SqlConnection(typeof(OutOfIDbModelFamily), typeof(DbContextBase<>));
        create.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(typeof(DbModel))]
    [InlineData(typeof(DbModelOfString))]
    [InlineData(typeof(DbModelOfInteger))]
    [InlineData(typeof(DbModelOfDouble))]
    [InlineData(typeof(DbModelOfLong))]
    [InlineData(typeof(DbModelOfGuid))]
    [InlineData(typeof(SqlDbModel))]
    [InlineData(typeof(ChildOfSqlDbModel))]
    [InlineData(typeof(ChildOfDbModel))]
    public void RegisterCollection_FromAnyKindsOfIDbModel_MustBeSuccessful(Type entityType)
    {
        var register = () =>
        {
            return new SqlConnectionStoreBuilder()
                .SetupDatabase(typeof(DbContextBase<>))
                .RegisterCollection(entityType);
        };
        register.Should().NotThrow();
    }

    [Fact]
    public void RegisterCollection_FromOutOfIDbModelFamily_MustThrowAnException()
    {
        var register = () =>
        {
            return new SqlConnectionStoreBuilder()
                .SetupDatabase(typeof(DbContextBase<>))
                .RegisterCollection(typeof(OutOfIDbModelFamily));
        };
        register.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion
}