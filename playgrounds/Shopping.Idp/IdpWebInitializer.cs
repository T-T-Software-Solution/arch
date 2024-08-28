using Microsoft.EntityFrameworkCore;
using Shopping.Idp.DbContexts;
using System.Reflection;
using TTSS.Core.AspNetCore;
using TTSS.Infra.Data.Sql;

namespace Shopping.Idp;

public class IdpWebInitializer : WebInitializerBase
{
    public override void RegisterOptions(IServiceCollection services)
    {
    }

    public override void RegisterServices(IServiceCollection services)
    {
        services
            .RegisterTTSSCoreHttp([Assembly.GetExecutingAssembly()])
            .RegisterIdentityServer<IdpDbContext>(new()
            {
                IsDevelopmentEnabled = true,
            });
    }

    public override void RegisterDatabases(IServiceCollection services)
    {
        base.RegisterDatabases(services);

        var dbConnection = Configuration?.GetConnectionString("DefaultConnection");
        services
            .SetupSqlDatabase(it => it.UseNpgsql(dbConnection).UseOpenIddict())
                .AddDbContext<IdpDbContext>()
            .Build();
    }
}