using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Contexts;
using TTSS.Core.Messaging.Pipelines;
using TTSS.Core.Models;

namespace TTSS.Core.Messaging;

public class IoCTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var mock = SetupMock<ITestInterface>();
        var behaviorTypes = new[]
        {
            typeof(IncrementPipelineBehavior<,>),
            typeof(MoreThan10PipelineBehavior<,>),
            typeof(AsyncIncrementPipelineBehavior<,>),
            typeof(AsyncMoreThan10PipelineBehavior<,>),
        };
        services
            .AddScoped<ICorrelationContext, MessagingContext>()
            .AddTransient<ITestInterface>(pvd => mock.Object)
            .RegisterMessagingModule([GetType().Assembly], behaviorTypes);
    }
}