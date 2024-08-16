using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Core.Services;
using TTSS.Infra.Data.Sql.DbContexte;
using TTSS.Infra.Data.Sql.Interceptors;

namespace TTSS.Infra.Data.Sql;

public class IoCTests : CommonTestCases
{
    public override bool IsManual => false;
    private SqliteConnection _connection = null!;

    protected override void RegisterServices(IServiceCollection services)
    {
        var connBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        _connection = new SqliteConnection(connBuilder.ConnectionString);

        services
            .AddSingleton<IDateTimeService>(DateTimeService)
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