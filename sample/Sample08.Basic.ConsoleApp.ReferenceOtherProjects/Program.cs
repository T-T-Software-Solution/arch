using Microsoft.Extensions.DependencyInjection;
using Sample01.Basic.ConsoleApp.Messages;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;

var relatedAssemblies = new[]
{
    typeof(Ping).Assembly,
    Assembly.GetExecutingAssembly(),
};

// Register TTSS Core
var services = new ServiceCollection();
services.RegisterTTSSCore(relatedAssemblies); // Load all handlers from the related assemblies

// Get the MessagingHub
var provider = services.BuildServiceProvider();
var hub = provider.GetRequiredService<IMessagingHub>();

// Send a message and get the response from the handler
var message = new Ping
{
    Name = "Alice"
};
var response = await hub.SendAsync(message);
Console.WriteLine(response.Response);

// Key takeaways from this example:
// 1. Referencing a C# project includes its NuGet packages automatically.
// 2. Include the referenced project's assembly to access its handlers.