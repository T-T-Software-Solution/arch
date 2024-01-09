using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data.Models;

namespace TTSS.Core.Data;

public class InMemoryRepositoryIoCTests : InMemoryRepositoryCommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        services.RegisterInMemoryRepository<BasicDbModel>();
        services.RegisterInMemoryRepository<CustomPrimaryKeyDbModel, int>();
    }
}