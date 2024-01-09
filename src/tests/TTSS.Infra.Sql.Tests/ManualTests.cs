using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.DbContexte;

namespace TTSS.Infra.Data.Sql;

public class ManualTests : CommonTestCases
{
    private SqliteConnection _connection = null!;

    protected override void RegisterServices(IServiceCollection services)
    {
        var store = new SqlConnectionStoreBuilder()
            .SetupDatabase<FruitDbContext>()
                .RegisterCollection<Apple>()
                .RegisterCollection<Banana>()
            .SetupDatabase<SchoolDbContext>()
                .RegisterCollection<Student>()
                .RegisterCollection<Teacher>()
                .RegisterCollection<Principal>()
            .Build();

        var lazyProvider = new Lazy<IServiceProvider>(() => services.BuildServiceProvider());
        var contextFactory = new SqlDbContextFactory(lazyProvider);
        var apple = new Lazy<SqlRepository<Apple>>(() => new SqlRepository<Apple>(store, contextFactory));
        var banana = new Lazy<SqlRepository<Banana>>(() => new SqlRepository<Banana>(store, contextFactory));
        var teacher = new Lazy<SqlRepository<Teacher>>(() => new SqlRepository<Teacher>(store, contextFactory));
        var student = new Lazy<SqlRepository<Student>>(() => new SqlRepository<Student>(store, contextFactory));
        var principal = new Lazy<SqlRepository<Principal, int>>(() => new SqlRepository<Principal, int>(store, contextFactory));

        var connBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        _connection = new SqliteConnection(connBuilder.ConnectionString);
        var assemblyName = Assembly.GetExecutingAssembly().FullName;
        services
            .AddScoped<SqlDbContextFactory>(_ => contextFactory)
            .AddScoped<Lazy<IServiceProvider>>(_ => lazyProvider)
            .AddDbContext<FruitDbContext>(it => it.UseSqlite(_connection, opt => opt.MigrationsAssembly(assemblyName)))
                .AddScoped<IRepository<Apple>>(_ => apple.Value)
                .AddScoped<IRepository<Apple, string>>(_ => apple.Value)
                .AddScoped<ISqlRepository<Apple>>(_ => apple.Value)
                .AddScoped<ISqlRepository<Apple, string>>(_ => apple.Value)
                .AddScoped<IRepository<Banana>>(_ => banana.Value)
                .AddScoped<IRepository<Banana, string>>(_ => banana.Value)
                .AddScoped<ISqlRepository<Banana>>(_ => banana.Value)
                .AddScoped<ISqlRepository<Banana, string>>(_ => banana.Value)
            .AddDbContext<SchoolDbContext>(it => it.UseSqlite(_connection, opt => opt.MigrationsAssembly(assemblyName)))
                .AddScoped<IRepository<Teacher>>(_ => teacher.Value)
                .AddScoped<IRepository<Teacher, string>>(_ => teacher.Value)
                .AddScoped<ISqlRepository<Teacher>>(p_vd => teacher.Value)
                .AddScoped<ISqlRepository<Teacher, string>>(_ => teacher.Value)
                .AddScoped<IRepository<Student>>(_ => student.Value)
                .AddScoped<IRepository<Student, string>>(_ => student.Value)
                .AddScoped<ISqlRepository<Student>>(_ => student.Value)
                .AddScoped<ISqlRepository<Student, string>>(_ => student.Value)
                .AddScoped<IRepository<Principal, int>>(_ => principal.Value)
                .AddScoped<ISqlRepository<Principal, int>>(_ => principal.Value);
    }

    public override void Dispose()
        => _connection.Dispose();
}