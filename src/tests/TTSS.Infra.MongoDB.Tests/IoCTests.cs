using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using TTSS.Core.Data;
using TTSS.Infra.Data.MongoDB.Documents;
using TTSS.Infra.Data.MongoDB.Repositories;

namespace TTSS.Infra.Data.MongoDB;

public class IoCTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        services
            .SetupMongoDatabase(Guid.NewGuid().ToString(), ConnectionString)
                .AddDbContext<TestDbContext>()
                .AddDbContext<SimplestTestDbContext>()
            .Build();
    }

    [Fact]
    public async Task MongoRepository_ShouldWorkProperlyOnIoC()
    {
        var services = new ServiceCollection();
        services
            .SetupMongoDatabase(Guid.NewGuid().ToString(), ConnectionString)
                .AddDbContext<TestDbContext>()
            .SetupMongoDatabase(Guid.NewGuid().ToString(), ConnectionString)
                .AddDbContext<SimplestTestDbContext>()
            .Build();
        var serviceProvider = services.BuildServiceProvider();

        var person = Fixture.Create<Person>();
        var sut = serviceProvider.GetService<IRepository<Person>>();
        await sut.InsertAsync(person);

        var repositories = new[]
        {
                serviceProvider.GetService<IRepository<Person>>(),
                serviceProvider.GetService<IRepository<Person, string>>(),
                serviceProvider.GetService<IMongoDbRepository<Person>>(),
                serviceProvider.GetService<IMongoDbRepository<Person, string>>(),
            };

        foreach (var repository in repositories)
        {
            var actual = await repository.GetByIdAsync(person.Id);
            actual.Should().BeEquivalentTo(person);
            repository.Get().Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task MultipleMongoRepositories_ShouldWorkProperlyOnIoC()
    {
        var firstDbRunner = MongoDbRunner.Start();
        var secondDbRunner = MongoDbRunner.Start();

        try
        {
            var services = new ServiceCollection();
            var dbName = Guid.NewGuid().ToString();
            services
                .SetupMongoDatabase(dbName, firstDbRunner.ConnectionString)
                    .AddDbContext<PersonDbContext>()
                .SetupMongoDatabase(dbName, secondDbRunner.ConnectionString)
                    .AddDbContext<StudentDbContext>()
                .Build();
            var serviceProvider = services.BuildServiceProvider();

            var person = Fixture.Create<Person>();
            var personRepo = serviceProvider.GetService<IRepository<Person, string>>();
            await personRepo.InsertAsync(person);
            var persons = await personRepo.GetByIdAsync(person.Id);
            persons.Should().BeEquivalentTo(person);
            personRepo.Get().Should().HaveCount(1);

            var student = Fixture.Create<Student>();
            var studentRepo = serviceProvider.GetService<IRepository<Student, string>>();
            await studentRepo.InsertAsync(student);
            var students = await studentRepo.GetByIdAsync(student.Id);
            students.Should().BeEquivalentTo(student);
            studentRepo.Get().Should().HaveCount(1);
        }
        finally
        {
            firstDbRunner.Dispose();
            secondDbRunner.Dispose();
        }
    }
}