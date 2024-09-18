using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample16.RemoteRequest.Shared.DbContexts;
using Sample16.RemoteRequest.Shared.Messages;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;
using TTSS.Infra.Data.Sql;

// INSTRUCTIONS:
// 1. Follow the instructions in Program.cs of Sample16.RemoteRequest.ConsoleApp1.

var app = await TTSSBuilder.BuildAsync(args, builder =>
{
    var relatedAssemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        Assembly.Load("Sample16.RemoteRequest.Shared"),
    };

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
            .AddDbContext<AnimalDbContext>()
        .Build();
});

// Create a timer to send a message every 2 seconds
var counter = 0;
var random = new Random();
var timer = new System.Timers.Timer(TimeSpan.FromSeconds(2));
timer.Elapsed += async (sender, e) =>
{
    var hub = app.ScopedServiceProvider.GetRequiredService<IMessagingHub>();
    var message = new Ping
    {
        First = ++counter,
        Second = random.Next(0, 10),
    };
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Send: {message}");
    Console.ForegroundColor = ConsoleColor.White;

    var response = await hub.SendAsync<Ping, Pong>(message);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Pong: {response}");
    Console.ForegroundColor = ConsoleColor.White;
};

app.AppStarted += (sender, e) =>
{
    Console.WriteLine("App 2 is ready");
    timer.Start();
};

app.Run();