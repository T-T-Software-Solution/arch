using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample05.Basic.ConsoleApp.WorkWithCorrelation.Handlers;

public sealed record FirstRequest : IRequesting;

file sealed class Handler(IMessagingHub hub, ICorrelationContext context) : RequestHandlerAsync<FirstRequest>
{
    public override async Task HandleAsync(FirstRequest request, CancellationToken cancellationToken)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"(ContextID: {context.CorrelationId}) from FirstRequest ({Console.ForegroundColor})");
        ShowContextBag("Before");
        context.ContextBag.Add("FirstRequest", "1");
        Console.ForegroundColor = ConsoleColor.White;

        await hub.SendAsync(new SecondRequest());

        Console.ForegroundColor = ConsoleColor.Red;
        ShowContextBag("After");
        Console.ForegroundColor = ConsoleColor.White;
    }

    private void ShowContextBag(string prefix)
    {
        Console.WriteLine(new string('-', 20));
        Console.WriteLine($"{prefix} ContextBag:");
        if (context.ContextBag.Count > 0)
        {
            foreach (var item in context.ContextBag.OrderBy(it => it.Key))
            {
                Console.WriteLine($"- Key: {item.Key},\tType: {item.Value.GetType().Name},\tValue: {item.Value}");
            }
        }
        else
        {
            Console.WriteLine("Empty");
        }
        Console.WriteLine(new string('-', 20));
    }
}