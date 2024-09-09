using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;
using TTSS.Core.Data;
using TTSS.Core.Hosting;

namespace TTSS.Core;

/// <summary>
/// Provides convenience methods for creating instances of <see cref="AppHost"/> with pre-configured defaults.
/// </summary>
public static class TTSSBuilder
{
    /// <summary>
    /// Builds an <see cref="AppHost"/> which hosts the application.
    /// </summary>
    /// <param name="args">The command line args</param>
    /// <param name="builder">The host application builder</param>
    /// <returns>The initialized <see cref="AppHost"/>.</returns>
    public static async Task<AppHost> BuildAsync(string[]? args, Action<HostApplicationBuilder>? builder = default)
    {
        var hostBuilder = CreateHostApplicationBuilder(args);
        RegisterActivitySource(hostBuilder.Services);
        builder?.Invoke(hostBuilder);

        var host = new AppHost(hostBuilder.Build());
        var warmups = host.ScopedServiceProvider.GetServices<IDbWarmup>();
        foreach (var item in warmups)
        {
            await item.WarmupAsync();
        }
        return host;
    }

    private static HostApplicationBuilder CreateHostApplicationBuilder(string[]? args)
    {
        HostApplicationBuilderSettings? settings = null;
#if DEBUG
        settings = new() { EnvironmentName = Environments.Development };
#endif
        return args?.Length != 0
            ? Host.CreateApplicationBuilder(args)
            : Host.CreateApplicationBuilder(settings);
    }

    private static void RegisterActivitySource(IServiceCollection services)
    {
        var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
        };
        ActivitySource.AddActivityListener(listener);
        var asm = Assembly.GetExecutingAssembly().GetName();
        var src = new ActivitySource(asm.Name!, asm.Version!.ToString());
        services.AddTransient(_ => src);
    }
}