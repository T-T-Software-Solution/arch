using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTSS.Infra.Loggings.OpenTelemetry;

namespace TTSS.Infra.Infra.Loggings;

public class IoCTests : CommonTestCases
{
    public override string ExpectedLogCategoryName => nameof(IoCTests);

    protected override void RegisterServices(IServiceCollection services)
    {
        var config = Fixture.Create<TTSS.Infra.Loggings.OpenTelemetry.Models.OTelConfiguration>();
        services.RegisterOpenTelemetry(config);
        services.AddTransient<ILoggerFactory>(pvd => LoggerFactoryMock.Object);
    }
}