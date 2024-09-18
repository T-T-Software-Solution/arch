using MassTransit;
using Microsoft.EntityFrameworkCore;
using Sample16.RemoteRequest.Shared;
using Sample16.RemoteRequest.Shared.DbContexts;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Web;
using TTSS.Infra.Data.Sql;

namespace Sample17.RemoteRequest.WebApp;

public sealed class MyWebInitializer : WebInitializerBase
{
    private string DbConnection => Configuration?.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string is not found.");

    public override void RegisterServices(IServiceCollection services)
    {
        var relatedAssemblies = new[]
        {
            Assembly.GetExecutingAssembly(),
            typeof(MappingProfileRegistrar).Assembly, // <-- Add assembly from class
        };

        // Regiser transportations
        services
            .AddOptions<SqlTransportOptions>()
            .Configure(option => option.ConnectionString = DbConnection);

        // Register services
        services
            .RegisterTTSSCoreHttp(relatedAssemblies)
            .AddPostgresMigrationHostedService()
            .RegisterRemoteRequest(relatedAssemblies, (bus, cfg) => bus.UsingPostgres(cfg));
    }

    public override void RegisterDatabases(IServiceCollection services)
    {
        // Register databases
        services
            .SetupSqlDatabase(it => it.UseNpgsql(DbConnection))
                .AddDbContext<AnimalDbContext>()
            .Build();
    }
}