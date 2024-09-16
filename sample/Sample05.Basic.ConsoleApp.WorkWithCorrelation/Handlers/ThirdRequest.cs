using Sample05.Basic.ConsoleApp.WorkWithCorrelation.Models;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample05.Basic.ConsoleApp.WorkWithCorrelation.Handlers;

public sealed record ThirdRequest(int Number) : IRequesting;

file sealed class Handler(ICorrelationContext context) : RequestHandler<ThirdRequest>
{
    public override void Handle(ThirdRequest request)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"(ContextID: {context.CorrelationId}) from ThirdRequest (Blue)");

        var valueFromTheSecondRequest = (int)context.ContextBag["SecondRequest"];
        Console.WriteLine($"Value from the SecondRequest {valueFromTheSecondRequest} + 5 is {valueFromTheSecondRequest + 5}");

        var animal = new Animal
        {
            Name = "Dog",
            Weight = new Random().Next(50, 100),
        };
        context.ContextBag.Add("ThirdRequest", animal);
        Console.ForegroundColor = ConsoleColor.White;
    }
}