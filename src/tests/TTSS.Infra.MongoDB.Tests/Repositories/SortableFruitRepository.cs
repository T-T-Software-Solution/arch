using TTSS.Core.Services;
using TTSS.Infra.Data.MongoDB.Documents;

namespace TTSS.Infra.Data.MongoDB.Repositories;

internal class SortableFruitRepository(MongoDbConnectionStore connectionStore, IMappingStrategy mappingStrategy) : MongoDbRepository<SortableFruit>(connectionStore, mappingStrategy)
{
}

internal class SortableFruitDbContext : IMongoDbContext
{
    public SortableFruitRepository SortableFruitRepository { get; set; }
}
