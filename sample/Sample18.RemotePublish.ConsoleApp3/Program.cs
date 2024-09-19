using Sample18.RemotePublish.Shared;
using System.Reflection;
using TTSS.Core;

// INSTRUCTIONS:
// 1. Follow the instructions in Program.cs of Sample18.RemotePublish.ConsoleApp1.

var app = await TTSSBuilder.BuildAsync(args, builder => builder.ConfigureCommon(Assembly.GetEntryAssembly()));

app.AppStarted += (sender, e) =>
{
    Console.WriteLine("App 3 is ready [SUBSCRIBER]");
};

app.Run();