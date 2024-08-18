using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Messaging.Pipelines;

namespace TTSS.Core.Messaging;

public class ManualTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var mock = SetupMock<ITestInterface>();
        services.AddTransient<ITestInterface>(pvd => mock.Object);
        services.AddSingleton<IMessagingHub>(sp => new MessagingHub(new Lazy<IServiceProvider>(sp)));
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies([typeof(ManualTests).Assembly]);
            var pipelines = new[]
            {
                    typeof(IncrementPipelineBehavior<,>),
                    typeof(MoreThan10PipelineBehavior<,>),
                    typeof(AsyncIncrementPipelineBehavior<,>),
                    typeof(AsyncMoreThan10PipelineBehavior<,>),
                };
            foreach (var item in pipelines)
            {
                cfg.AddOpenBehavior(item, ServiceLifetime.Scoped);
            }
        });
    }
}