using TTSS.Core.Services;
using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB.Repositories;

internal class OrderablePersonRepository(MongoDbConnectionStore connectionStore, IMappingStrategy mappingStrategy) : MongoDbRepository<OrderablePerson>(connectionStore, mappingStrategy)
{
}

internal class OrderablePersonDbContext : IMongoDbContext
{
    public OrderablePersonRepository OrderablePersonRepository { get; set; }
}
