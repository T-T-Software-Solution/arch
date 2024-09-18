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
// 1. Start the PostgreSQL server. (Update PostgreSQL connection string in appsettings.json if needed)
// 2. Run Sample16.RemoteRequest.ConsoleApp1.
// 3. Run Sample16.RemoteRequest.ConsoleApp2.
// 4. Watch the console output.
// 5. Close ConsoleApp2 for 10 seconds, then reopen it.
// 6. (Optional) Comment out timer.Start() or increase the interval to observe changes.

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
var timer = new System.Timers.Timer(TimeSpan.FromSeconds(2));
timer.Elapsed += async (sender, e) =>
{
    var hub = app.ScopedServiceProvider.GetRequiredService<IMessagingHub>();
    Console.ForegroundColor = ConsoleColor.Green;
    var message = new Greeting($"Hello {++counter}, this text from App1");
    Console.WriteLine($"Send: {message}");
    await hub.SendAsync(message);
};

app.AppStarted += (sender, e) =>
{
    Console.WriteLine("App 1 is ready");
    timer.Start();
};

app.Run();

// Key takeaways from this example:
// 1. TTSS.Core.Remoting allows applications to communicate as if they're within the same app.
// 2. Use the same message types in a shared project to enable this feature.
// 3. Use IRemoteRequesting<T> for cross-app communication instead of IRequest<T>.
// 4. Use RemoteRequestHandlerAsync<T> to handle messages from other apps.
// 5. MessageBroker handles traffic between apps via the database.
// 6. Even if an application is down, the message will be delivered once it's back online.
// 7. This feature requires additional setup for the database and message broker.