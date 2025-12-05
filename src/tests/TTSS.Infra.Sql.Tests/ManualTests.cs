using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TTSS.Core.Data;
using TTSS.Core.Models;
using TTSS.Core.Services;
using TTSS.Infra.Data.Sql;
using TTSS.Infra.Data.Sql.DbContexte;
using TTSS.Infra.Data.Sql.DbModels;
using TTSS.Infra.Data.Sql.Interceptors;

namespace TTSS.Infra.Data.Sql;

public class ManualTests : CommonTestCases
{
    protected override bool IsManual => true;
    private SqliteConnection _connection = null!;

    protected override void RegisterServices(IServiceCollection services)
    {
        var interceptorBuilder = SqlInterceptorBuilder.Default
            .Register<TestSqlInterceptorManual>();

        var store = new SqlConnectionStoreBuilder()
            .SetupDatabase<FruitDbContext>()
                .RegisterCollection<Apple>()
                .RegisterCollection<Banana>()
                .RegisterCollection<OrderableFruit>()
            .SetupDatabase<SchoolDbContext>()
                .RegisterCollection<Student>()
                .RegisterCollection<Teacher>()
                .RegisterCollection<Principal>()
            .SetupDatabase<SpaceDbContext>()
                .RegisterCollection<Astronaut>()
                .RegisterCollection<Spaceship>()
                .RegisterCollection<AuditLog>()
                .RegisterCollection<SensitivitySpaceStation>()
                .RegisterCollection<MaintenanceLog>()
                .RegisterCollection<SeriousLog>()
            .Build(interceptorBuilder);

        var config = new Mapper(new MapperConfiguration(cfg => { }));
        var mappingStrategy = new AutoMapperMappingStrategy(config);
        var lazyProvider = new Lazy<IServiceProvider>(() => services.BuildServiceProvider());
        var contextFactory = new SqlDbContextFactory(lazyProvider);
        var apple = new Lazy<SqlRepository<Apple>>(() => new SqlRepository<Apple>(store, contextFactory, mappingStrategy));
        var banana = new Lazy<SqlRepository<Banana>>(() => new SqlRepository<Banana>(store, contextFactory, mappingStrategy));
        var teacher = new Lazy<SqlRepository<Teacher>>(() => new SqlRepository<Teacher>(store, contextFactory, mappingStrategy));
        var student = new Lazy<SqlRepository<Student>>(() => new SqlRepository<Student>(store, contextFactory, mappingStrategy));
        var principal = new Lazy<SqlRepository<Principal, int>>(() => new SqlRepository<Principal, int>(store, contextFactory, mappingStrategy));
        var astronaut = new Lazy<SqlRepository<Astronaut>>(() => new SqlRepository<Astronaut>(store, contextFactory, mappingStrategy));
        var spaceship = new Lazy<SqlRepository<Spaceship>>(() => new SqlRepository<Spaceship>(store, contextFactory, mappingStrategy));
        var audit = new Lazy<SqlRepository<AuditLog>>(() => new SqlRepository<AuditLog>(store, contextFactory, mappingStrategy));
        var sensitivitySpaceStation = new Lazy<SqlRepository<SensitivitySpaceStation>>(() => new SqlRepository<SensitivitySpaceStation>(store, contextFactory, mappingStrategy));
        var maintenanceLog = new Lazy<SqlRepository<MaintenanceLog>>(() => new SqlRepository<MaintenanceLog>(store, contextFactory, mappingStrategy));
        var seriousLog = new Lazy<SqlRepository<SeriousLog>>(() => new SqlRepository<SeriousLog>(store, contextFactory, mappingStrategy));
        var orderableFruit = new Lazy<SqlRepository<OrderableFruit>>(() => new SqlRepository<OrderableFruit>(store, contextFactory, mappingStrategy));

        var connBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        _connection = new SqliteConnection(connBuilder.ConnectionString);
        var assemblyName = Assembly.GetExecutingAssembly().FullName;
        services
            .AddScoped<IDbWarmup, FruitDbContext>()
            .AddScoped<IDbWarmup, SchoolDbContext>()
            .AddScoped<IDbWarmup, SpaceDbContext>()
            .AddSingleton<IDateTimeService>(DateTimeService)
            .AddScoped<SqlDbContextFactory>(_ => contextFactory)
            .AddScoped<Lazy<IServiceProvider>>(_ => lazyProvider)
            .AddScoped<ICorrelationContext, CorrelationContext>(_ => Context)
            .AddInterceptors(store, interceptorBuilder)
            .AddDbContext<FruitDbContext>(it => it.UseSqlite(_connection, opt => opt.MigrationsAssembly(assemblyName)))
                .AddScoped<IRepository<Apple>>(_ => apple.Value)
                .AddScoped<IRepository<Apple, string>>(_ => apple.Value)
                .AddScoped<ISqlRepository<Apple>>(_ => apple.Value)
                .AddScoped<ISqlRepository<Apple, string>>(_ => apple.Value)
                .AddScoped<IRepository<Banana>>(_ => banana.Value)
                .AddScoped<IRepository<Banana, string>>(_ => banana.Value)
                .AddScoped<ISqlRepository<Banana>>(_ => banana.Value)
                .AddScoped<ISqlRepository<Banana, string>>(_ => banana.Value)
                .AddScoped<IRepository<OrderableFruit>>(_ => orderableFruit.Value)
            .AddDbContext<SchoolDbContext>(it => it.UseSqlite(_connection, opt => opt.MigrationsAssembly(assemblyName)))
                .AddScoped<IRepository<Teacher>>(_ => teacher.Value)
                .AddScoped<IRepository<Teacher, string>>(_ => teacher.Value)
                .AddScoped<ISqlRepository<Teacher>>(_ => teacher.Value)
                .AddScoped<ISqlRepository<Teacher, string>>(_ => teacher.Value)
                .AddScoped<IRepository<Student>>(_ => student.Value)
                .AddScoped<IRepository<Student, string>>(_ => student.Value)
                .AddScoped<ISqlRepository<Student>>(_ => student.Value)
                .AddScoped<ISqlRepository<Student, string>>(_ => student.Value)
                .AddScoped<IRepository<Principal, int>>(_ => principal.Value)
                .AddScoped<ISqlRepository<Principal, int>>(_ => principal.Value)
            .AddDbContext<SpaceDbContext>(it => it.UseSqlite(_connection, opt => opt.MigrationsAssembly(assemblyName)))
                .AddScoped<IRepository<Astronaut>>(_ => astronaut.Value)
                .AddScoped<IRepository<Astronaut, string>>(_ => astronaut.Value)
                .AddScoped<ISqlRepository<Astronaut>>(_ => astronaut.Value)
                .AddScoped<ISqlRepository<Astronaut, string>>(_ => astronaut.Value)
                .AddScoped<IRepository<Spaceship>>(_ => spaceship.Value)
                .AddScoped<IRepository<Spaceship, string>>(_ => spaceship.Value)
                .AddScoped<ISqlRepository<Spaceship>>(_ => spaceship.Value)
                .AddScoped<ISqlRepository<Spaceship, string>>(_ => spaceship.Value)
                .AddScoped<IRepository<AuditLog>>(_ => audit.Value)
                .AddScoped<IRepository<AuditLog, string>>(_ => audit.Value)
                .AddScoped<ISqlRepository<AuditLog>>(_ => audit.Value)
                .AddScoped<ISqlRepository<AuditLog, string>>(_ => audit.Value)
                .AddScoped<IRepository<SensitivitySpaceStation>>(_ => sensitivitySpaceStation.Value)
                .AddScoped<IRepository<SensitivitySpaceStation, string>>(_ => sensitivitySpaceStation.Value)
                .AddScoped<ISqlRepository<SensitivitySpaceStation>>(_ => sensitivitySpaceStation.Value)
                .AddScoped<ISqlRepository<SensitivitySpaceStation, string>>(_ => sensitivitySpaceStation.Value)
                .AddScoped<IRepository<MaintenanceLog>>(_ => maintenanceLog.Value)
                .AddScoped<IRepository<MaintenanceLog, string>>(_ => maintenanceLog.Value)
                .AddScoped<ISqlRepository<MaintenanceLog>>(_ => maintenanceLog.Value)
                .AddScoped<ISqlRepository<MaintenanceLog, string>>(_ => maintenanceLog.Value)
                .AddScoped<IRepository<SeriousLog>>(_ => seriousLog.Value)
                .AddScoped<IRepository<SeriousLog, string>>(_ => seriousLog.Value)
                .AddScoped<ISqlRepository<SeriousLog>>(_ => seriousLog.Value)
                .AddScoped<ISqlRepository<SeriousLog, string>>(_ => seriousLog.Value);
    }

    public override void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}