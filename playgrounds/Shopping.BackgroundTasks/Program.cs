using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shopping.Shared;
using Shopping.Shared.DbContexts;
using Shopping.Shared.Interceptors;
using System.Reflection;
using TTSS.Core;
using TTSS.Infra.Data.Sql;

var app = await TTSSBuilder.BuildAsync(args, builder =>
{
    var assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        typeof(MappingProfileRegistrar).Assembly,
    };

    var dbConnection = builder.Configuration?.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string is not found.");

    // Register services
    builder.Services
        .RegisterTTSSCore(assemblies)
        .RegisterRemoteRequest(dbConnection, assemblies);

    // Register databases
    builder.Services
        .SetupSqlDatabase(it => it.UseNpgsql(dbConnection))
            .AddDbContext<ShoppingDbContext>()
        .Build(cfg => cfg.Register<AuditInterceptor>());
});

var logger = app.GetLogger<Program>();
logger.LogInformation("Application started");
app.Run();