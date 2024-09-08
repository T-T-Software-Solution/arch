using TTSS.Core.Configurations;
using TTSS.Core.Data;
using TTSS.Core.Web.Pipelines;

namespace TTSS.Core.Web;

/// <summary>
/// Web application initializer.
/// </summary>
public abstract class WebInitializerBase
{
    #region Fields

    private readonly List<IValidator> _registeredOptionsValidators = [];
    private readonly Dictionary<Type, ServiceDescriptor> _registeredMiddlewares = [];

    #endregion

    #region Properties

    /// <summary>
    /// The web application builder.
    /// </summary>
    internal WebApplicationBuilder? Builder { get; set; }

    /// <summary>
    /// A builder for web applications and services.
    /// </summary>
    protected WebApplicationBuilder? WebApplicationBuilder => Builder;

    /// <summary>
    /// Represents a set of key/value application configuration properties.
    /// </summary>
    protected IConfiguration? Configuration => WebApplicationBuilder?.Configuration;

    /// <summary>
    /// Provides information about the web hosting environment an application is running in.
    /// </summary>
    protected IWebHostEnvironment? Environment => WebApplicationBuilder?.Environment;

    #endregion

    #region Methods

    /// <summary>
    /// Pre-build actions.
    /// </summary>
    public virtual void PreBuild()
    {
    }

    /// <summary>
    /// Pre-build asynchronous actions.
    /// </summary>
    public virtual Task PreBuildAsync()
        => Task.CompletedTask;

    /// <summary>
    /// Post-build actions.
    /// </summary>
    /// <param name="app">The web application used to configure the HTTP pipeline, and routes</param>
    public virtual void PostBuild(WebApplication app)
    {
    }

    /// <summary>
    /// Post-build asynchronous  actions.
    /// </summary>
    /// <param name="app">The web application used to configure the HTTP pipeline, and routes</param>
    public virtual async Task PostBuildAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProviderScope = scope.ServiceProvider;
        var warmups = serviceProviderScope.GetServices<IDbWarmup>();
        foreach (var item in warmups)
        {
            await item.WarmupAsync();
        }
    }

    /// <summary>
    /// Registers databases into the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection</param>
    public virtual void RegisterDatabases(IServiceCollection services)
    {
    }

    /// <summary>
    /// Configures the <see cref="IApplicationBuilder"/> to specify how the application will respond to HTTP requests.
    /// </summary>
    /// <param name="builder">The application builder</param>
    public virtual void ConfigurePipelines(IApplicationBuilder builder)
    {
    }

    /// <summary>
    /// Configures the <see cref="IEndpointRouteBuilder"/> to specify the routes for an application.
    /// </summary>
    /// <param name="builder">The route builder</param>
    public virtual void ConfigureRoutes(IEndpointRouteBuilder builder)
    {
    }

    /// <summary>
    /// Registers monitoring into the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection</param>
    public virtual void RegisterSystemMonitorings(IServiceCollection services)
    {
    }

    /// <summary>
    /// Registers services into the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection</param>
    public virtual void RegisterOptions(IServiceCollection services)
    {
    }

    /// <summary>
    /// Registers services into the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection</param>
    public abstract void RegisterServices(IServiceCollection services);

    /// <summary>
    /// Gets the options validator.
    /// </summary>
    /// <typeparam name="TOptionValidator">Type of the options validator</typeparam>
    /// <returns>Options validator</returns>
    protected TOptionValidator? GetOptions<TOptionValidator>()
        where TOptionValidator : IOptionsValidator
        => _registeredOptionsValidators.OfType<TOptionValidator>().FirstOrDefault();

    /// <summary>
    /// Adds options validator to the registeration list.
    /// </summary>
    /// <param name="validator">The options validator</param>
    /// <exception cref="ArgumentNullException">The validator is required</exception>
    internal void AddOptionsValidator(IOptionsValidator? validator)
        => _registeredOptionsValidators.Add(validator ?? throw new ArgumentNullException(nameof(validator)));

    internal WebInitializerBase AddMiddleware<TMiddleware>(ServiceDescriptor descriptor)
        where TMiddleware : IMiddleware
    {
        var target = typeof(TMiddleware);
        if (false == _registeredMiddlewares.ContainsKey(target))
        {
            _registeredMiddlewares.Add(target, descriptor);
        }
        return this;
    }

    internal void RegisterMiddlewares(IServiceCollection services)
    {
        this.RegisterMiddleware<HttpCorrelationContextMiddleware>(ServiceLifetime.Scoped);

        foreach (var item in _registeredMiddlewares)
        {
            services.Add(item.Value);
        }
    }

    internal void UseMiddlewares(IApplicationBuilder builder)
    {
        foreach (var item in _registeredMiddlewares)
        {
            builder.UseMiddleware(item.Key);
        }
    }

    #endregion
}