using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data;
using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB;

public class ManualTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var store = new MongoDbConnectionStoreBuilder()
            .SetupDatabase(Guid.NewGuid().ToString(), ConnectionString)
                .RegisterCollection<Person>()
                .RegisterCollection<Student>()
            .Build();
        var personRepository = new MongoDbRepository<Person>(store);
        var studentRepository = new MongoDbRepository<Student>(store);
        var personPrimitiveRepository = new MongoDbRepository<Person, string>(store, it => it.Id);
        var studentPrimitiveRepository = new MongoDbRepository<Student, string>(store, it => it.Id);
        services
            .AddSingleton<IRepository<Person>>(personRepository)
            .AddSingleton<IRepository<Student>>(studentRepository)
            .AddSingleton<IRepository<Person, string>>(personPrimitiveRepository)
            .AddSingleton<IRepository<Student, string>>(studentPrimitiveRepository)
            .AddSingleton<IMongoDbRepository<Person>>(personRepository)
            .AddSingleton<IMongoDbRepository<Student>>(studentRepository)
            .AddSingleton<IMongoDbRepository<Person, string>>(personPrimitiveRepository)
            .AddSingleton<IMongoDbRepository<Student, string>>(studentPrimitiveRepository);
    }

    #region Insert

    [Theory]
    [InlineAutoData(1, "One")]
    [InlineAutoData(2, "")]
    [InlineAutoData(3, null)]
    public async Task Insert_WithDefaultCollectionName_ShouldSaveDataAsItBe(string id, string name)
    {
        var store = new MongoDbConnectionStoreBuilder()
            .SetupDatabase(Guid.NewGuid().ToString(), ConnectionString)
                .RegisterCollection<Person>(noDiscriminator: true)
            .Build();
        var sut = new MongoDbRepository<Person>(store);
        await InsertAndValidate(new Person
        {
            Id = id,
            Name = name,
        }, sut, 1);
    }

    [Theory]
    [InlineAutoData(1, "One")]
    [InlineAutoData(2, "")]
    [InlineAutoData(3, null)]
    public async Task Insert_WithCustomCollectionName_ShouldSaveDataAsItBe(string id, string name)
    {
        var store = new MongoDbConnectionStoreBuilder()
            .SetupDatabase(Guid.NewGuid().ToString(), ConnectionString)
                .RegisterCollection<Person>("simple", noDiscriminator: true)
            .Build();
        var sut = new MongoDbRepository<Person>(store);
        await InsertAndValidate(new Person
        {
            Id = id,
            Name = name,
        }, sut, 1);
    }

    [Fact]
    public async Task Insert_Discriminator_ShouldSaveDataAsItBe()
    {
        var collectionName = Guid.NewGuid().ToString();
        var store = new MongoDbConnectionStoreBuilder()
            .SetupDatabase(Guid.NewGuid().ToString(), ConnectionString)
                .RegisterCollection<Person>(collectionName)
                .RegisterCollection<Student>(collectionName, isChild: true)
            .Build();

        var simpleSut = new MongoDbRepository<Person>(store);
        var simpleData = Fixture.Create<Person>();
        await InsertAndValidate(simpleData, simpleSut, 1);

        var studentSut = new MongoDbRepository<Student>(store);
        var studentData = Fixture.Create<Student>();
        await InsertAndValidate(studentData, studentSut, 1);
    }

    private static async Task InsertAndValidate<T>(T data, MongoDbRepository<T> sut, int expectedCount)
        where T : IDbModel<string>
    {
        await sut.InsertAsync(data);
        var actual = await sut.GetByIdAsync(data.Id);
        actual.Should().BeEquivalentTo(data);
        sut.Get().Should().HaveCount(expectedCount);
    }

    #endregion
}