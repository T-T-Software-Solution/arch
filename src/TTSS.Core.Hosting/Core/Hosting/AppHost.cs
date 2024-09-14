using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TTSS.Core.Messaging;

namespace TTSS.Core.Hosting;

/// <summary>
/// Represents the application host.
/// </summary>
/// <param name="host">The host</param>
public sealed class AppHost(IHost host)
{
    #region Properties

    /// <summary>
    /// Scoped service provider.
    /// </summary>
    public IServiceProvider ScopedServiceProvider => host.Services.CreateScope().ServiceProvider;

    /// <summary>
    /// Messaging hub.
    /// </summary>
    public IMessagingHub MessagingHub => ScopedServiceProvider.GetRequiredService<IMessagingHub>();

    #endregion

    #region Methods

    /// <summary>
    /// Runs an application and block the calling thread until host shutdown.
    /// The ScopedServiceProvider instance is disposed of after running.
    /// </summary>
    public void Run()
        => host.Run();

    /// <summary>
    /// Runs an application and returns a <see cref="Task"/> that only completes when the token is triggered or shutdown is triggered.
    /// The ScopedServiceProvider instance is disposed of after running.
    /// </summary>
    public Task RunAsync()
        => host.RunAsync();

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
    public ILogger GetLogger([CallerMemberName] string? callerName = default)
        => ScopedServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(callerName);

    #endregion
}