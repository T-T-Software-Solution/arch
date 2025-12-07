using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data;
using TTSS.Core.Services;
using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB;

public class ManualTests : CommonTestCases
{
    private AutoMapperMappingStrategy _mappingStrategy;

    protected override void RegisterServices(IServiceCollection services)
    {
        var store = new MongoDbConnectionStoreBuilder()
            .SetupDatabase(Guid.NewGuid().ToString(), ConnectionString)
                .RegisterCollection<Person>()
                .RegisterCollection<Student>()
                .RegisterCollection<OrderablePerson>()
                .RegisterCollection<SortableFruit>()
            .Build();

        var config = new Mapper(new MapperConfiguration(cfg => { }));
        _mappingStrategy = new AutoMapperMappingStrategy(config);
        var personRepository = new MongoDbRepository<Person>(store, _mappingStrategy);
        var studentRepository = new MongoDbRepository<Student>(store, _mappingStrategy);
        var personPrimitiveRepository = new MongoDbRepository<Person, string>(store, _mappingStrategy, it => it.Id);
        var studentPrimitiveRepository = new MongoDbRepository<Student, string>(store, _mappingStrategy, it => it.Id);
        var orderablePersonRepository = new MongoDbRepository<OrderablePerson>(store, _mappingStrategy);
        var sortableFruitRepository = new MongoDbRepository<SortableFruit>(store, _mappingStrategy);
        services
            .AddSingleton<IRepository<Person>>(personRepository)
            .AddSingleton<IRepository<Student>>(studentRepository)
            .AddSingleton<IRepository<Person, string>>(personPrimitiveRepository)
            .AddSingleton<IRepository<Student, string>>(studentPrimitiveRepository)
            .AddSingleton<IMongoDbRepository<Person>>(personRepository)
            .AddSingleton<IMongoDbRepository<Student>>(studentRepository)
            .AddSingleton<IMongoDbRepository<Person, string>>(personPrimitiveRepository)
            .AddSingleton<IMongoDbRepository<Student, string>>(studentPrimitiveRepository)
            .AddSingleton<IRepository<OrderablePerson>>(orderablePersonRepository)
            .AddSingleton<IRepository<SortableFruit>>(sortableFruitRepository);
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
        var sut = new MongoDbRepository<Person>(store, _mappingStrategy);
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
        var sut = new MongoDbRepository<Person>(store, _mappingStrategy);
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

        var simpleSut = new MongoDbRepository<Person>(store, _mappingStrategy);
        var simpleData = Fixture.Create<Person>();
        await InsertAndValidate(simpleData, simpleSut, 1);

        var studentSut = new MongoDbRepository<Student>(store, _mappingStrategy);
        var studentData = Fixture.Create<Student>();
        await InsertAndValidate(studentData, studentSut, 1);
    }

    private static async Task InsertAndValidate<TEntity>(TEntity data, MongoDbRepository<TEntity> sut, int expectedCount)
        where TEntity : class, IDbModel<string>
    {
        await sut.InsertAsync(data);
        var actual = await sut.GetByIdAsync(data.Id);
        actual.Should().BeEquivalentTo(data);
        sut.Get().Should().HaveCount(expectedCount);
    }

    #endregion
}