using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Core.IdentityServer;
using TTSS.Core.IdentityServer.Configurations;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TTSS.Core.AspNetCore;

/// <summary>
/// Helper extension methods for the register TTSS IdentityServer services.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Add the identity server modules.
    /// </summary>
    /// <typeparam name="TIdentityDbContext">The identity database context</typeparam>
    /// <param name="target">The service collection</param>
    /// <param name="options">The identity configuration options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterIdentityServer<TIdentityDbContext>(this IServiceCollection target, IdentityConOptions? options = default)
        where TIdentityDbContext : DbContext, IIdentityDbContext
        => RegisterIdentityServer<TIdentityDbContext, IdentityUser>(target, options);

    /// <summary>
    /// Add the identity server modules.
    /// </summary>
    /// <typeparam name="TIdentityDbContext">The identity database context</typeparam>
    /// <typeparam name="TIdentityUser">The type representing a User in the system</typeparam>
    /// <param name="target">The service collection</param>
    /// <param name="options">The identity configuration options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterIdentityServer<TIdentityDbContext, TIdentityUser>(this IServiceCollection target, IdentityConOptions? options = default)
        where TIdentityDbContext : DbContext, IIdentityDbContext
        where TIdentityUser : class
    {
        target
            .AddMvc();

        target
            .Configure<IdentityOptions>(cfg =>
            {
                cfg.ClaimsIdentity.RoleClaimType = Claims.Role;
                cfg.ClaimsIdentity.EmailClaimType = Claims.Email;
                cfg.ClaimsIdentity.UserNameClaimType = Claims.Name;
                cfg.ClaimsIdentity.UserIdClaimType = Claims.Subject;
            })
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cfg =>
            {
                cfg.LoginPath = "/Identity/Account/Login";
                cfg.LogoutPath = "/Identity/Account/Logout";
                cfg.AccessDeniedPath = "/Identity/Account/AccessDenied";
                cfg.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options?.CookieAuthenticationOptions?.Invoke(cfg);
            });

        if (options?.IsDevelopmentEnabled ?? false)
        {
            target.AddDatabaseDeveloperPageExceptionFilter();
        }

        var identityBuilder = target
            .AddIdentity<TIdentityUser, IdentityRole>(cfg => options?.IdentityOptions?.Invoke(cfg))
            .AddEntityFrameworkStores<TIdentityDbContext>()
            .AddDefaultTokenProviders();

        options?.IdentityBuilder?.Invoke(identityBuilder);

        target
            .AddOpenIddict()
            .AddCore(cfg =>
            {
                cfg
                .UseEntityFrameworkCore()
                .UseDbContext<TIdentityDbContext>();
                options?.OpenIddictCoreBuilder?.Invoke(cfg);
            })
            .AddServer(cfg =>
            {
                var accessTokenLifetime = options is null
                    ? TimeSpan.FromHours(IdentityConOptions.DefaultAccessTokenLifetime)
                    : TimeSpan.FromSeconds(options.AccessTokenLifetimeInSeconds);
                var refreshTokenLifetime = options is null
                    ? TimeSpan.FromHours(IdentityConOptions.DefaultRefreshTokenLifetime)
                    : TimeSpan.FromSeconds(options.RefreshTokenLifetimeInSeconds);
                cfg
                .SetAuthorizationEndpointUris("/connect/authorize")
                .SetTokenEndpointUris("/connect/token")
                .SetUserinfoEndpointUris("/connect/userinfo")
                .SetLogoutEndpointUris("/connect/logout")
                .AllowAuthorizationCodeFlow()
                .RequireProofKeyForCodeExchange()
                .RegisterScopes(options?.SupportedScopes?.ToArray() ?? IdentityConOptions.DefaultScopes)


                .AllowRefreshTokenFlow()
                .DisableSlidingRefreshTokenExpiration()
                .SetAccessTokenLifetime(accessTokenLifetime)
                .SetRefreshTokenLifetime(refreshTokenLifetime)

                .UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
                .EnableTokenEndpointPassthrough()
                .EnableUserinfoEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .EnableStatusCodePagesIntegration();

                if (options?.IsDevelopmentEnabled ?? false)
                {
                    cfg
                    .DisableAccessTokenEncryption() // HACK: disable the encryption to allow the token to be read by the client.
                    .AddDevelopmentSigningCertificate()
                    .AddDevelopmentEncryptionCertificate();
                }
                else
                {
                    var certificate = options?.Certificate?.Invoke();
                    ArgumentNullException.ThrowIfNull(certificate);
                    cfg
                    .AddEncryptionCertificate(certificate)
                    .AddSigningCertificate(certificate);
                }

                options?.OpenIddictServerBuilder?.Invoke(cfg);
            })
            .AddValidation(cfg =>
            {
                cfg.UseLocalServer();
                cfg.UseAspNetCore();
                options?.OpenIddictValidationBuilder?.Invoke(cfg);
            });

        return target;
    }
}