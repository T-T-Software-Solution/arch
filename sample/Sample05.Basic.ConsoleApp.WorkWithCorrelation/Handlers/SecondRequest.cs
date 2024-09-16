using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample05.Basic.ConsoleApp.WorkWithCorrelation.Handlers;

public sealed record SecondRequest : IRequesting;

file sealed class Handler(IMessagingHub hub, ICorrelationContext context) : RequestHandlerAsync<SecondRequest>
{
    public override async Task HandleAsync(SecondRequest request, CancellationToken cancellationToken)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"(ContextID: {context.CorrelationId}) from SecondRequest (Green)");
        context.ContextBag.Add("SecondRequest", new Random().Next(0, 50));

        await hub.SendAsync(new ThirdRequest(8));
    }
}