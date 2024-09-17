using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample10.Basic.ConsoleApp.LongRunningTask.Handlers;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;

// You can set the environment variable here
//Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");

var app = await TTSSBuilder.BuildAsync(args, builder =>
{
    // Register services
    builder.Services
        .RegisterTTSSCore([Assembly.GetExecutingAssembly()]);

    var secretValue = builder.Configuration["MySecret"];
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"The secret is {secretValue}");
    Console.WriteLine($"Current environment is {builder.Environment.EnvironmentName}");
    Console.ForegroundColor = ConsoleColor.White;
});

// Create a timer to send a message every 2 seconds
var timer = new System.Timers.Timer(TimeSpan.FromSeconds(2));
timer.Elapsed += async (sender, e) =>
{
    var hub = app.ScopedServiceProvider.GetRequiredService<IMessagingHub>();
    await hub.SendAsync(new EchoRequest("Hello, World!"));
};

app.AppStarted += (sender, e) =>
{
    // Get the ILogger service and write a log
    var logger = app.GetLogger<Program>();
    logger.LogInformation("Application started hurayyyyyyyy !"); // If you don't see this log, uncomment the line 9 above.

    // Start the timer
    timer.Start();
};

app.Run();

// Key takeaways from this example:
// 1. TTSSBuilder creates a long-running task with configurable DI.
// 2. Services like ILogger, IConfiguration, and IMetricsBuilder are available.
// 3. The environment variable is set in the code at line 9.
// 4. appsettings.json is auto-selected based on the environment.