using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Contexts;
using TTSS.Core.Messaging.Pipelines;

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
            .AddTransient<ITestInterface>(pvd => mock.Object)
            .RegisterTTSSCore<MessagingContext>([GetType().Assembly], behaviorTypes);
    }
}