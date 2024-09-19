using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sample18.RemotePublish.Shared;
using Sample18.RemotePublish.Shared.Messages;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;

// INSTRUCTIONS:
// 1. Start the PostgreSQL server. (Update PostgreSQL connection string in appsettings.json if needed)
// 2. Run Sample18.RemotePublish.ConsoleApp 2,3,4.
// 4. Watch the console output.
// 5. Close ConsoleApp2 for 10 seconds, then reopen it.

var app = await TTSSBuilder.BuildAsync(args, builder => builder.ConfigureCommon(Assembly.GetEntryAssembly()));

// Create a timer to send a message every 2 seconds
var counter = 0;
var random = new Random();
var timer = new System.Timers.Timer(TimeSpan.FromSeconds(2));
timer.Elapsed += async (sender, e) =>
{
    var hub = app.ScopedServiceProvider.GetRequiredService<IMessagingHub>();
    Console.ForegroundColor = ConsoleColor.Green;
    var message = new NotifyUserRegistered
    {
        Id = Guid.NewGuid().ToString(),
        Username = $"UserName{++counter}",
        Role = (UserRole)random.Next(0, 4),
    };
    Console.WriteLine($"Publish: {message}");
    await hub.PublishAsync(message);
};

app.AppStarted += (sender, e) =>
{
    Console.WriteLine("App 1 is ready [PUBLISHER]");
    timer.Start();
};

app.Run();

// Key takeaways from this example:
// 1. IRemotePublication is used to publish messages to multiple handlers at the same time.
// 2. Use the same message types in a shared project to enable this feature.
// 3. Use IRemotePublication for cross-app communication instead of IPublication.
// 4. Use RemotePublicationHandlerAsync<T> to handle messages from other apps.
// 5. Even if an application is down, the message will be delivered once it's back online.
// 6. Reduce setup time by registering in the shared project.