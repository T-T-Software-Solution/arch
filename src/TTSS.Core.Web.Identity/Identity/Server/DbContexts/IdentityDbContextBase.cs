using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TTSS.Core.Data;

namespace TTSS.Core.Web.Identity.Server.DbContexts;

// TODO: Duplicated code with DbContextBase, consider refactoring to a common service.
/// <summary>
/// Identity database context.
/// </summary>
/// <typeparam name="TUser">The type of the user objects</typeparam>
public abstract class IdentityDbContextBase<TUser>(DbContextOptions options)
    : IdentityDbContext<TUser>(options), IDbWarmup
    where TUser : IdentityUser
{
    #region Fields

    private IEnumerable<IInterceptor>? _interceptors;

    #endregion

    #region Methods

    async Task IDbWarmup.WarmupAsync()
    {
        await Database.EnsureCreatedAsync();
        await Database.ExecuteSqlRawAsync("SELECT 1"); // TODO: TBD alternative query
    }

    /// <summary>
    /// Configures the schema needed for the identity framework.
    /// </summary>
    /// <param name="builder">
    /// The builder being used to construct the model for this context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseOpenIddict();
    }

    /// <summary>
    /// Configure the database (and other options) to be used for this context.
    /// </summary>
    /// <param name="optionsBuilder">A builder used to create or modify options for this context</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_interceptors?.Any() ?? false)
        {
            optionsBuilder.AddInterceptors(_interceptors);
        }
        base.OnConfiguring(optionsBuilder);
    }

    internal void SetInterceptors(IEnumerable<IInterceptor> interceptors)
        => _interceptors ??= interceptors;

    #endregion
}

/// <summary>
/// Identity database context.
/// </summary>
public abstract class IdentityDbContextBase(DbContextOptions options) : IdentityDbContextBase<IdentityUser>(options);