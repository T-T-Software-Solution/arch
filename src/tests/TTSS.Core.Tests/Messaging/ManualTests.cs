using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Contexts;
using TTSS.Core.Messaging.Pipelines;

namespace TTSS.Core.Messaging;

public class ManualTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var pipelines = new[]
        {
            typeof(IncrementPipelineBehavior<,>),
            typeof(MoreThan10PipelineBehavior<,>),
            typeof(AsyncIncrementPipelineBehavior<,>),
            typeof(AsyncMoreThan10PipelineBehavior<,>),
        };
        var mock = SetupMock<ITestInterface>();
        services
            .RegisterTTSSCore<MessagingContext>([typeof(ManualTests).Assembly], pipelines)
            .AddTransient<ITestInterface>(pvd => mock.Object);
    }
}