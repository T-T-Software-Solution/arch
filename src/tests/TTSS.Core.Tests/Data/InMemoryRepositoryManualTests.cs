using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TTSS.Core.Data.Models;
using TTSS.Core.Services;

namespace TTSS.Core.Data;

public class InMemoryRepositoryManualTests : InMemoryRepositoryCommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        base.RegisterServices(services);
        var mapper = new Mock<IMapper>();
        var mappingStrategy = new AutoMapperMappingStrategy(mapper.Object);
        var basicRepository = new InMemoryRepository<BasicDbModel>(mappingStrategy, it => it.Id);
        services.AddSingleton<IRepository<BasicDbModel>>(_ => basicRepository);

        var customPrimaryKeyRepository = new InMemoryRepository<CustomPrimaryKeyDbModel, int>(mappingStrategy, it => it.Id);
        services.AddSingleton<IRepository<CustomPrimaryKeyDbModel, int>>(_ => customPrimaryKeyRepository);

        var orderableRepository = new InMemoryRepository<OrderableDbModel, int>(mappingStrategy, it => it.Id);
        services.AddSingleton<IRepository<OrderableDbModel, int>>(_ => orderableRepository);

        var sortableRepository = new InMemoryRepository<SortableFruit>(mappingStrategy, it => it.Id);
        services.AddSingleton<IRepository<SortableFruit>>(_ => sortableRepository);
    }
}
