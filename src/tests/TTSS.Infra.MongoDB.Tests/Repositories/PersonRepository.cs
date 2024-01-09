using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB.Repositories;

internal class PersonRepository : MongoDbRepository<Person, string>
{
    public PersonRepository(MongoDbConnectionStore connectionStore) : base(connectionStore)
    {
    }
}

internal class PersonDbContext : IMongoDbContext
{
    public PersonRepository PersonRepository { get; set; }
}