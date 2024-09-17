using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sample15.Basic.WebApp.WorkWithDatabase.DbContexts;
using System.Reflection;
using TTSS.Core.Web;
using TTSS.Infra.Data.Sql;

namespace Sample15.Basic.WebApp.WorkWithDatabase;

public class DemoWebInitializer : WebInitializerBase
{
    public override void RegisterServices(IServiceCollection services)
    {
        services
            .RegisterTTSSCoreHttp([Assembly.GetExecutingAssembly()]);
    }

    public override void RegisterDatabases(IServiceCollection services)
    {
        base.RegisterDatabases(services);
        var connBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        var connection = new SqliteConnection(connBuilder.ConnectionString);
        services
            .SetupSqlDatabase(it => it.UseSqlite(connection, opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)))
                .AddDbContext<UniversityDbContext>()
            .Build();
    }
}