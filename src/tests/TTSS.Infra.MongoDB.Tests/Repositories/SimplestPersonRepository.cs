using TTSS.Core.Services;
using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB.Repositories;

internal class SimplestPersonRepository(MongoDbConnectionStore connectionStore, IMappingStrategy mappingStrategy) : MongoDbRepository<Person>(connectionStore, mappingStrategy)
{
}

internal class SimplestTestDbContext : IMongoDbContext
{
    public SimplestPersonRepository SimplestPersonRepository { get; set; }
}
