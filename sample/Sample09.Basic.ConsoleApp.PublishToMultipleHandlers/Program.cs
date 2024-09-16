using Microsoft.Extensions.DependencyInjection;
using Sample09.Basic.ConsoleApp.PublishToMultipleHandlers.Messages;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;

// Register TTSS Core
var services = new ServiceCollection();
services.RegisterTTSSCore([Assembly.GetExecutingAssembly()]);

// Get the MessagingHub
var provider = services.BuildServiceProvider();
var hub = provider.GetRequiredService<IMessagingHub>();

// Publish a message to multiple handlers
var random = new Random();
var message = new NotifyWeatherChanged
{
    NewValue = random.Next(-20, 100),
    FromValue = random.Next(-20, 100),
};
await hub.PublishAsync(message);

// Key takeaways from this example:
// 1. Publish differs from Send by delivering a message to multiple handlers.