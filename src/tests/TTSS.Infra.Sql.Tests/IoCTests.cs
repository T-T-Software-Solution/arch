using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Models;
using TTSS.Infra.Data.Sql.DbContexte;
using TTSS.Infra.Data.Sql.Interceptors;

namespace TTSS.Infra.Data.Sql;

public class IoCTests : CommonTestCases
{
    protected override bool IsManual => false;
    private SqliteConnection _connection = null!;

    protected override void RegisterServices(IServiceCollection services)
    {
        var connBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        _connection = new SqliteConnection(connBuilder.ConnectionString);

        services
            .RegisterTTSSCore([Assembly.GetExecutingAssembly()])
            .AddScoped<ICorrelationContext, CorrelationContext>(_ => Context)
            .SetupSqlDatabase(it => it.UseSqlite(_connection, opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)))
                .AddDbContext<FruitDbContext>()
                .AddDbContext<SchoolDbContext>()
                .AddDbContext<SpaceDbContext>()
            .Build(cfg => cfg.Register<TestSqlInterceptorIoC>());
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}