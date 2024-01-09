using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB.Repositories;

internal class StudentDbContext : IMongoDbContext
{
    public StudentRepository StudentRepository { get; set; }
}

internal class StudentRepository : MongoDbRepository<Student, string>
{
    public StudentRepository(MongoDbConnectionStore connectionStore) : base(connectionStore)
    {
    }
}

internal class TestDbContext : IMongoDbContext
{
    public PersonRepository PersonRepository { get; set; }
    public StudentRepository StudentRepository { get; set; }
}