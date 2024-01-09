using TTSS.Core.Configurations;

namespace TTSS.Core.AspNetCore;

/// <summary>
/// Helper extension methods for <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Builds the web application and runs the initializer.
    /// </summary>
    /// <typeparam name="TWebInitializer">Web application initializer</typeparam>
    /// <param name="target">Web application builder</param>
    /// <returns>The web application</returns>
    /// <exception cref="InvalidOperationException">The builder is required</exception>
    public static async Task<WebApplication> BuildAsync<TWebInitializer>(this WebApplicationBuilder target)
        where TWebInitializer : WebInitializerBase
    {
        var initializer = Activator.CreateInstance<TWebInitializer>()
            ?? throw new InvalidOperationException($"Could not create instance of {typeof(TWebInitializer)}");

        initializer.Builder = target;
        initializer.PreBuild();
        await initializer.PreBuildAsync();
        initializer.RegisterOptions(target.Services);
        initializer.RegisterServices(target.Services);
        initializer.RegisterDatabases(target.Services);

        var app = target.Build();
        initializer.ConfigureRoutes(app);
        initializer.ConfigurePipelines(app);
        initializer.PostBuild(app);
        await initializer.PostBuildAsync(app);
        return app;
    }

    /// <summary>
    /// Adds options validator to the service collection.
    /// </summary>
    /// <typeparam name="TOptionsValidator">Option validator type</typeparam>
    /// <param name="target">Web application builder</param>
    /// <param name="failureMessage">Failure message</param>
    /// <returns>Web application builder</returns>
    public static WebApplicationBuilder AddOptionsValidator<TOptionsValidator>(this WebApplicationBuilder target, string? failureMessage = default)
        where TOptionsValidator : class, IOptionsValidator
    {
        target.Services.AddOptionsValidator<TOptionsValidator>(target.Configuration, failureMessage);
        return target;
    }

    /// <summary>
    /// Adds options validator to the service collection.
    /// </summary>
    /// <typeparam name="TOptionsValidator">Option validator type</typeparam>
    /// <param name="target">Web application builder</param>
    /// <param name="failureMessage">Failure message</param>
    /// <returns>Web application builder</returns>
    public static WebInitializerBase AddOptionsValidator<TOptionsValidator>(this WebInitializerBase target, string? failureMessage = default)
        where TOptionsValidator : class, IOptionsValidator
    {
        var services = target.Builder?.Services ?? throw new InvalidOperationException("The builder is required");
        var configuration = target.Builder?.Configuration ?? throw new InvalidOperationException("The configuration is required");
        services.AddOptionsValidator<TOptionsValidator>(configuration!, failureMessage);
        var validator = configuration.GetSection(TOptionsValidator.SectionName).Get<TOptionsValidator>();
        target.AddOptionsValidator(validator);
        return target;
    }

    /// <summary>
    /// Adds options validator to the service collection.
    /// </summary>
    /// <typeparam name="TOptionsValidator">Option validator type</typeparam>
    /// <param name="target">Web application builder</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="failureMessage">Failure message</param>
    /// <returns>Web application builder</returns>
    internal static IServiceCollection AddOptionsValidator<TOptionsValidator>(this IServiceCollection target, IConfiguration configuration, string? failureMessage = default)
        where TOptionsValidator : class, IOptionsValidator
    {
        target
            .AddOptions<TOptionsValidator>()
            .Bind(configuration.GetSection(TOptionsValidator.SectionName))
            .ValidateDataAnnotations()
            .Validate(it => it.Validate(), failureMessage ?? $"Options validation failed for {typeof(TOptionsValidator).Name}.")
            .ValidateOnStart();
        return target;
    }
}