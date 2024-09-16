using Microsoft.Extensions.DependencyInjection;
using Sample06.Basic.ConsoleApp.CustomCorrelationContext.Handlers;
using Sample06.Basic.ConsoleApp.CustomCorrelationContext.Models;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;

// Register TTSS Core
var services = new ServiceCollection();
services.RegisterTTSSCore<DemoContext>([Assembly.GetExecutingAssembly()]); // <-- Specify the custom context here

// Create service provider
var provider = services.BuildServiceProvider();

// Send a message to the handler
var hub = provider.GetRequiredService<IMessagingHub>();
var context = await hub.SendAsync(new AssignNameAndScore(Guid.NewGuid().ToString()));

Console.WriteLine($"Name: {context.Name}");
Console.WriteLine($"Score: {context.Score}");
Console.WriteLine($"Value: {context.Value}");
Console.WriteLine($"MyDog: {context.MyDog?.Name}");

// Key takeaways from this example:
// 1. Everything is the same as the previous example, but with a custom DemoContext.
// 2. Create your own context by inheriting from CorrelationContext, accessible to all handlers.