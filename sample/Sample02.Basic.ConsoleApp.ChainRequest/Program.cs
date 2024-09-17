using Microsoft.Extensions.DependencyInjection;
using Sample02.Basic.ConsoleApp.ChainRequest.Messages;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Messaging;

// Register TTSS Core
var services = new ServiceCollection();
services.RegisterTTSSCore([Assembly.GetExecutingAssembly()]);

// Get the MessagingHub
var provider = services.BuildServiceProvider();
var hub = provider.GetRequiredService<IMessagingHub>();

// Send a message and get the response from the handler
var message = new CalculateRequest
{
    Operand1 = 5,
    Operand2 = 3,
    Operator = CalculationOperator.Multiply,
};
var response = await hub.SendAsync(message);
Console.WriteLine(response);

// Key takeaways from this example:
// 1. Handlers can call other handlers via the MessagingHub.