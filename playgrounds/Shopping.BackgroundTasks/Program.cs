using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopping.BackgroundTasks.Biz;
using Shopping.Shared.DbContexts;
using Shopping.Shared.Interceptors;
using System.Reflection;
using TTSS.Core;
using TTSS.Infra.Data.Sql;

var app = await TTSSBuilder.BuildAsync(args, builder =>
{
    var dbConnection = builder.Configuration?.GetConnectionString("DefaultConnection");

    builder.Services
        .RegisterTTSSCore([Assembly.GetExecutingAssembly()])
        .SetupSqlDatabase(it => it.UseNpgsql(dbConnection))
            .AddDbContext<ShoppingDbContext>()
        .Build(cfg => cfg.Register<AuditInterceptor>());
});

await app.MessagingHub.SendAsync(new CheckStock());

app.Run();