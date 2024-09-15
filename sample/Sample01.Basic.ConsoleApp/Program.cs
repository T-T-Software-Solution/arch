using Microsoft.Extensions.DependencyInjection;
using Sample01.Basic.ConsoleApp.Messages;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;

// Register TTSS Core
var services = new ServiceCollection();
services.RegisterTTSSCore([Assembly.GetExecutingAssembly()]);

// Get the MessagingHub
var provider = services.BuildServiceProvider();
var hub = provider.GetRequiredService<IMessagingHub>();

// Send a message to the handler
var message1 = new Greeting
{
    Message = "Hello, World!"
};
await hub.SendAsync(message1);

// Send a message and get the response from the handler
var message2 = new Ping
{
    Name = "Alice"
};
var response = await hub.SendAsync(message2);
Console.WriteLine(response.Response);