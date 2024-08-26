using TTSS.Core.Services;
using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB.Repositories;

internal class StudentDbContext : IMongoDbContext
{
    public StudentRepository StudentRepository { get; set; }
}

internal class StudentRepository(MongoDbConnectionStore connectionStore, IMappingStrategy mappingStrategy) : MongoDbRepository<Student, string>(connectionStore, mappingStrategy)
{
}

internal class TestDbContext : IMongoDbContext
{
    public PersonRepository PersonRepository { get; set; }
    public StudentRepository StudentRepository { get; set; }
}