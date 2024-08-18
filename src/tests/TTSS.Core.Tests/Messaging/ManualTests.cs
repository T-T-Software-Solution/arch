using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Contexts;
using TTSS.Core.Messaging.Pipelines;
using TTSS.Core.Models;

namespace TTSS.Core.Messaging;

public class ManualTests : CommonTestCases
{
    protected override void RegisterServices(IServiceCollection services)
    {
        var mock = SetupMock<ITestInterface>();
        services
            .AddScoped<ICorrelationContext, MessagingContext>()
            .AddTransient<ITestInterface>(pvd => mock.Object)
            .AddSingleton<IMessagingHub>(sp => new MessagingHub(new Lazy<IServiceProvider>(sp)))
            .AddMediatR(cfg =>
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