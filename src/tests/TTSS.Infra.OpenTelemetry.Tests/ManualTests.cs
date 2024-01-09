using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using TTSS.Core.Loggings;

namespace TTSS.Infra.Infra.Loggings;
public class ManualTests : CommonTestCases
{
    public override string ExpectedLogCategoryName => nameof(ManualTests);

    protected override void RegisterServices(IServiceCollection services)
    {
        services.ConfigureOpenTelemetryTracerProvider((sp, builder) =>
        {
            var sourceName = Fixture.Create<string>();
            builder.AddSource(sourceName);
            builder.ConfigureResource(cfg => cfg.AddAttributes(new Dictionary<string, object>
            {
                { "service.instance.id", Fixture.Create<string>() },
                { "service.name", Fixture.Create<string>() },
                { "service.namespace", Fixture.Create<string>() },
            }));
        });
        var oTelBuilder = services.AddOpenTelemetry();
        services.AddTransient<IActivityFactory, ActivityFactory>();
        services.AddSingleton<ActivitySource>(pvd => new(Fixture.Create<string>()));
        services.AddTransient<ILoggerFactory>(pvd => LoggerFactoryMock.Object);
    }
}
