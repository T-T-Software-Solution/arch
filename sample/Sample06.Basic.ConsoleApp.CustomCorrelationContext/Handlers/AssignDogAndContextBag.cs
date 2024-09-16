using Sample06.Basic.ConsoleApp.CustomCorrelationContext.Models;
using TTSS.Core.Messaging;
using TTSS.Core.Messaging.Handlers;
using TTSS.Core.Models;

namespace Sample06.Basic.ConsoleApp.CustomCorrelationContext.Handlers;

public sealed record AssignDogAndContextBag : IRequesting;

file sealed class Handler(ICorrelationContext context) : RequestHandler<AssignDogAndContextBag>
{
    public override void Handle(AssignDogAndContextBag request)
    {
        if (context is not DemoContext customCtx)
        {
            throw new InvalidOperationException("Invalid context type.");
        }

        customCtx.MyDog = new Dog { Name = "Rex", Age = 5 };
        context.ContextBag.Add("MyKey", "MyValue");
    }
}