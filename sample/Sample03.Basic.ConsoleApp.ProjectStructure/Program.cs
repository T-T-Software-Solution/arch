using Microsoft.Extensions.DependencyInjection;
using Sample03.Basic.ConsoleApp.ProjectStructure.Calculators;
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
var message = new Calculator
{
    Operand1 = 5,
    Operand2 = 3,
    Operator = CalculationOperator.Divide,
};
var response = await hub.SendAsync(message);
Console.WriteLine(response);

// Key takeaways from this example:
// 1. The project is organized by modules like Calculators.
// 2. Each module's content is in the same folder.
// 3. Messages and handlers are in the same file.
// 4. Handlers are called via MessagingHub, not directly, so they are file-scoped.