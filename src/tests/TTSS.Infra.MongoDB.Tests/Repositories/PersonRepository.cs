using TTSS.Core.Services;
using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB.Repositories;

internal class PersonRepository(MongoDbConnectionStore connectionStore, IMappingStrategy mappingStrategy) : MongoDbRepository<Person, string>(connectionStore, mappingStrategy)
{
}

internal class PersonDbContext : IMongoDbContext
{
    public PersonRepository PersonRepository { get; set; }
}