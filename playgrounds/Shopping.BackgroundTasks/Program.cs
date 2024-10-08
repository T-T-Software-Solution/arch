﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shopping.Shared;
using Shopping.Shared.DbContexts;
using Shopping.Shared.Interceptors;
using Shopping.Shared.Requests.Learns;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;
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

    builder.Services
            .AddOptions<SqlTransportOptions>()
            .Configure(option => option.ConnectionString = dbConnection);

    // Register services
    builder.Services
        .RegisterTTSSCore(assemblies)
        .AddPostgresMigrationHostedService()
        .RegisterRemoteRequest(assemblies, (bus, configure) => bus.UsingPostgres(configure));

    // Register databases
    builder.Services
        .SetupSqlDatabase(it => it.UseNpgsql(dbConnection))
            .AddDbContext<ShoppingDbContext>()
        .Build(cfg => cfg.Register<AuditInterceptor>());
});

app.AppStarted += async (sender, e) =>
{
    var logger = app.GetLogger<Program>();
    logger.LogInformation("Application started");

    var hub = app.ScopedServiceProvider.GetRequiredService<IMessagingHub>();
    await hub.SendAsync(new Greeting { Message = "Hello, A" });
    await hub.SendAsync(new Greeting { Message = "Hello, B" });
    await hub.SendAsync(new Greeting { Message = "Hello, C" });
    var rsp = await hub.SendAsync<Ping, Pong>(new Ping(3, 7));
    logger.LogInformation("The result from the remote request is {@Result}", rsp.Result);
};

app.Run();