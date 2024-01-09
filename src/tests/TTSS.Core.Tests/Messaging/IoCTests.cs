using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Messaging.Pipelines;

namespace TTSS.Core.Messaging;

public class IoCTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var mock = SetupMock<ITestInterface>();
        services.AddTransient<ITestInterface>(pvd => mock.Object);
        services.RegisterMessagingModule(new[] { GetType().Assembly }, new[]
        {
            typeof(IncrementPipelineBehavior<,>),
            typeof(MoreThan10PipelineBehavior<,>),
            typeof(AsyncIncrementPipelineBehavior<,>),
            typeof(AsyncMoreThan10PipelineBehavior<,>),
        });
    }
}