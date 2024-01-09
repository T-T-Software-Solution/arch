using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB.Repositories;

internal class SimplestPersonRepository : MongoDbRepository<Person>
{
    public SimplestPersonRepository(MongoDbConnectionStore connectionStore) : base(connectionStore)
    {
    }
}

internal class SimplestTestDbContext : IMongoDbContext
{
    public SimplestPersonRepository SimplestPersonRepository { get; set; }
}
