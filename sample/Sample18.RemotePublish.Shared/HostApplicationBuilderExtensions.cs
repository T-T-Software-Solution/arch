using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample18.RemotePublish.Shared.DbContexts;
using System.Reflection;
using TTSS.Core;
using TTSS.Infra.Data.Sql;

namespace Sample18.RemotePublish.Shared;

public static class HostApplicationBuilderExtensions
{
    public static void ConfigureCommon(this HostApplicationBuilder builder, Assembly? runningAssembly)
    {
        Assembly[] relatedAssemblies =
        [
            runningAssembly!,
            typeof(HostApplicationBuilderExtensions).Assembly,
        ];

        // Regiser transportations
        var dbConnection = builder.Configuration?.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string is not found.");
        builder.Services
            .AddOptions<SqlTransportOptions>()
            .Configure(option => option.ConnectionString = dbConnection);

        // Register services
        builder.Services
            .RegisterTTSSCore(relatedAssemblies)
            .AddPostgresMigrationHostedService()
            .RegisterRemoteRequest(relatedAssemblies, (bus, cfg) => bus.UsingPostgres(cfg));

        // Register databases
        builder.Services
            .SetupSqlDatabase(it => it.UseNpgsql(dbConnection))
                .AddDbContext<TransportDbContext>()
            .Build();
    }
}