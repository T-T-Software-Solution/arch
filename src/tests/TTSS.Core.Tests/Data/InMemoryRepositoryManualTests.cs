using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data.Models;

namespace TTSS.Core.Data;

public class InMemoryRepositoryManualTests : InMemoryRepositoryCommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var basicRepository = new InMemoryRepository<BasicDbModel>(it => it.Id);
        services.AddSingleton<IRepository<BasicDbModel>>(_ => basicRepository);

        var customPrimaryKeyRepository = new InMemoryRepository<CustomPrimaryKeyDbModel, int>(it => it.Id);
        services.AddSingleton<IRepository<CustomPrimaryKeyDbModel, int>>(_ => customPrimaryKeyRepository);
    }
}
