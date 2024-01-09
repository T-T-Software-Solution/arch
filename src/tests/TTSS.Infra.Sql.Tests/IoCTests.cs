using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Infra.Data.Sql.DbContexte;

namespace TTSS.Infra.Data.Sql;

public class IoCTests : CommonTestCases
{
    private SqliteConnection _connection = null!;

    protected override void RegisterServices(IServiceCollection services)
    {
        var connBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        _connection = new SqliteConnection(connBuilder.ConnectionString);

        services
            .SetupSqlDatabase(it => it.UseSqlite(_connection, opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)))
                .AddDbContext<FruitDbContext>()
                .AddDbContext<SchoolDbContext>()
            .Build();
    }

    public override void Dispose()
        => _connection.Dispose();
}