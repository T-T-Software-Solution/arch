using Sample01.Basic.ConsoleApp.Messages;
using TTSS.Core.Messaging.Handlers;

namespace Sample01.Basic.ConsoleApp.Handlers;

internal sealed class GreetingHandler : RequestHandler<Greeting>
{
    public override void Handle(Greeting request)
    {
        Console.WriteLine($"Received: {request.Message}");
    }
}