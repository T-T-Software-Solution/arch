using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TTSS.Core.Messaging;

namespace TTSS.Core.Hosting;

/// <summary>
/// Represents the application host.
/// </summary>
public sealed class AppHost
{
    #region Fields

    private readonly IHost _host;

    #endregion

    #region Properties

    /// <summary>
    /// Scoped service provider.
    /// </summary>
    public IServiceProvider ScopedServiceProvider => _host.Services.CreateScope().ServiceProvider;

    /// <summary>
    /// Messaging hub.
    /// </summary>
    public IMessagingHub MessagingHub => ScopedServiceProvider.GetRequiredService<IMessagingHub>();

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the application has started.
    /// </summary>
    public event EventHandler? AppStarted;

    /// <summary>
    /// Occurs when the application has stopped.
    /// </summary>
    public event EventHandler? AppStopped;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AppHost"/> class.
    /// </summary>
    /// <param name="host">Application host</param>
    internal AppHost(IHost host)
    {
        _host = host;
        var lifetime = ScopedServiceProvider.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() => AppStarted?.Invoke(this, EventArgs.Empty));
        lifetime.ApplicationStopped.Register(() => AppStopped?.Invoke(this, EventArgs.Empty));
    }

    #endregion

    #region Methods

    /// <summary>
    /// Runs an application and block the calling thread until host shutdown.
    /// The ScopedServiceProvider instance is disposed of after running.
    /// </summary>
    public void Run()
        => _host.Run();

    /// <summary>
    /// Runs an application and returns a <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.
    /// The ScopedServiceProvider instance is disposed of after running.
    /// </summary>
    public Task RunAsync()
        => _host.RunAsync();

    /// <summary>
    /// Gets a logger instance.
    /// </summary>
    /// <typeparam name="TCategoryName">The category name</typeparam>
    /// <returns>Returns the logger instance</returns>
    public ILogger<TCategoryName> GetLogger<TCategoryName>()
        => ScopedServiceProvider.GetRequiredService<ILogger<TCategoryName>>();

    /// <summary>
    /// Gets a logger instance.
    /// </summary>
    /// <param name="callerName">The caller name</param>
    /// <returns>Returns the logger instance</returns>
    public ILogger GetLogger([CallerMemberName] string callerName = default!)
        => ScopedServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(callerName);

    #endregion
}