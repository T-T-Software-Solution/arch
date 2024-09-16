using Microsoft.Extensions.DependencyInjection;
using Sample05.Basic.ConsoleApp.WorkWithCorrelation.Handlers;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;
using TTSS.Core.Models;

// Register TTSS Core
var services = new ServiceCollection();
services.RegisterTTSSCore([Assembly.GetExecutingAssembly()]);

// Create service provider
var rootProvider = services.BuildServiceProvider();

// Send a message to the handler (Scope 1)
Console.WriteLine("Round 1");
var hub1 = rootProvider
    .CreateScope() // <-- Create a 1st scope
    .ServiceProvider
    .GetRequiredService<IMessagingHub>();
await hub1.SendAsync(new FirstRequest());

Console.WriteLine();

// Send a message to the handler (Scope 2)
Console.WriteLine("Round 2");
var hub2 = rootProvider
    .CreateScope() // <-- Create a 2nd scope
    .ServiceProvider
    .GetRequiredService<IMessagingHub>();
await hub2.SendAsync(new FirstRequest());

// Key takeaways from this example:
// 1. Each request has its own correlation context.
// 2. The correlation context is shared within the same scope.
// 3. The correlation context is not shared across different scopes.
// 4. The correlation bag stores data shared between requests.