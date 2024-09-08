using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using TTSS.Core.Data;
using TTSS.Core.Web.Identity.Client.Configurations;
using TTSS.Core.Web.Identity.Server.Configurations;
using TTSS.Core.Web.Identity.Server.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TTSS.Core.Web;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
    public static IServiceCollection RegisterIdentityServer<TIdentityDbContext>(this IServiceCollection target, IdentityServerOptions? options = default)
        where TIdentityDbContext : DbContext, IDbWarmup
        => RegisterIdentityServer<TIdentityDbContext, IdentityUser>(target, options);

    /// <summary>
    /// Add the identity server modules.
    /// </summary>
    /// <typeparam name="TIdentityDbContext">The identity database context</typeparam>
    /// <typeparam name="TIdentityUser">The type representing a User in the system</typeparam>
    /// <param name="target">The service collection</param>
    /// <param name="options">The identity configuration options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterIdentityServer<TIdentityDbContext, TIdentityUser>(this IServiceCollection target, IdentityServerOptions? options = default)
        where TIdentityDbContext : DbContext, IDbWarmup
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
                cfg.LoginPath = "/identity/account/login";
                cfg.LogoutPath = "/identity/account/logout";
                cfg.AccessDeniedPath = "/identity/account/accessdenied";
                cfg.ExpireTimeSpan = IdentityServerOptions.GetDuration(options?.RefreshTokenLifetime, TimeSpan.FromHours(IdentityServerOptions.DefaultRefreshTokenLifetime));
                options?.CookieAuthenticationOptions?.Invoke(cfg);
            });

        if (options?.IsDevelopmentEnabled ?? false)
        {
            target.AddDatabaseDeveloperPageExceptionFilter();
        }

        var identityBuilder = target
            .AddIdentity<TIdentityUser, IdentityRole>(cfg =>
            {
                if (options?.IsDevelopmentEnabled ?? false)
                {
                    cfg.Password.RequiredLength = 4;
                    cfg.Password.RequireDigit = false;
                    cfg.Password.RequireLowercase = false;
                    cfg.Password.RequireUppercase = false;
                    cfg.Password.RequireNonAlphanumeric = false;
                    cfg.SignIn.RequireConfirmedAccount = false;
                }
                options?.IdentityOptions?.Invoke(cfg);
            })
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
                var accessTokenLifetime = IdentityServerOptions.GetDuration(options?.AccessTokenLifetime, TimeSpan.FromHours(IdentityServerOptions.DefaultAccessTokenLifetime));
                var refreshTokenLifetime = IdentityServerOptions.GetDuration(options?.RefreshTokenLifetime, TimeSpan.FromHours(IdentityServerOptions.DefaultRefreshTokenLifetime));

                cfg
                .SetAuthorizationEndpointUris("/connect/authorize")
                .SetTokenEndpointUris("/connect/token")
                .SetUserinfoEndpointUris("/connect/userinfo")
                .SetLogoutEndpointUris("/connect/logout")
                .AllowAuthorizationCodeFlow()
                .RequireProofKeyForCodeExchange()
                .RegisterScopes(options?.SupportedScopes?.ToArray() ?? IdentityServerOptions.DefaultScopes)

                .AllowRefreshTokenFlow()

                .SetAccessTokenLifetime(accessTokenLifetime)
                .SetRefreshTokenLifetime(refreshTokenLifetime)
                .DisableSlidingRefreshTokenExpiration()

                .UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
                .EnableTokenEndpointPassthrough()
                .EnableUserinfoEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .EnableStatusCodePagesIntegration();

                if (options?.IsDevelopmentEnabled ?? false)
                {
                    cfg
                    .AddEphemeralSigningKey()
                    .AddEphemeralEncryptionKey()
                    .AddDevelopmentSigningCertificate()
                    .AddDevelopmentEncryptionCertificate()
                    .DisableAccessTokenEncryption();
                }
                else
                {
                    var certificate = options?.Certificate?.Invoke()
                    ?? throw new InvalidOperationException("The certificate is required for production environment.");

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

    /// <summary>
    /// Add the identity client modules.
    /// </summary>
    /// <typeparam name="TIdentityDbContext">The database context that will be used to store the tokens</typeparam>
    /// <param name="target">The service collection</param>
    /// <param name="options">The identity configuration options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterIdentityClient<TIdentityDbContext>(this IServiceCollection target, IdentityClientOptions options)
         where TIdentityDbContext : DbContext, IDbWarmup
    {
        ArgumentNullException.ThrowIfNull(options);
        var webKey = GetWebKey();
        ArgumentNullException.ThrowIfNull(webKey);

        target
           .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(cfg =>
           {
               cfg.Authority = options.AuthorityBaseUrl;
               cfg.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = false, // TODO: Validate audience
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = cfg.Authority, // TODO: Support multiple issuers
                   IssuerSigningKeys = webKey.GetSigningKeys(),
               };
               options.BearerOptions?.Invoke(cfg);
           });

        target
            .AddOpenIddict()
            .AddCore(it => it.UseEntityFrameworkCore().UseDbContext<TIdentityDbContext>())
            .AddClient(cfg =>
            {
                var registration = new OpenIddictClientRegistration
                {
                    ClientId = options.ClientId,
                    ClientSecret = options.ClientSecret,
                    ProviderName = options.ProviderName,
                    Issuer = new(options.AuthorityBaseUrl, UriKind.Absolute),
                    RedirectUri = new(options.LoginCallbackPath ?? IdentityClientRegistrarOptions.DefaultLoginCallbackPath, UriKind.Relative),
                    PostLogoutRedirectUri = new(options.LogoutCallbackPath ?? IdentityClientRegistrarOptions.DefaultLogoutCallbackPath, UriKind.Relative),
                };

                foreach (var item in options.Scopes) registration.Scopes.Add(item);
                foreach (var item in options.GrantTypes) registration.GrantTypes.Add(item);
                foreach (var item in options.ResponseTypes) registration.ResponseTypes.Add(item);

                cfg
                .AllowAuthorizationCodeFlow()
                .AddRegistration(registration)
                .UseAspNetCore()
                .EnableStatusCodePagesIntegration()
                .EnableRedirectionEndpointPassthrough()
                .EnablePostLogoutRedirectionEndpointPassthrough();

                var httpBuilder = cfg
                .UseSystemNetHttp()
                .SetProductInformation(options.GetType().Assembly);

                if (options.IsDevelopmentEnabled)
                {
                    httpBuilder
                    .ConfigureHttpClientHandler(hdl =>
                    {
                        hdl.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    });

                    cfg
                    .AddEphemeralEncryptionKey()
                    .AddEphemeralSigningKey()
                    .AddDevelopmentSigningCertificate()
                    .AddDevelopmentEncryptionCertificate();
                }
                else
                {
                    var credentialKey = options.CredentialKey?.Invoke()
                    ?? throw new InvalidOperationException("The credential key is required for production environment.");

                    cfg
                    .AddEncryptionKey(credentialKey)
                    .AddSigningKey(credentialKey);
                }
            });

        return target;

        JsonWebKeySet? GetWebKey()
        {
            if (string.IsNullOrWhiteSpace(options.AuthorityBaseUrl))
            {
                return null;
            }

            // HACK: TBD, This is a temporary solution to get the web key.
            var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var discoveryTask = Task.Run(async () => await http.GetDiscoveryDocumentAsync(options.AuthorityBaseUrl));
            var discovery = discoveryTask.GetAwaiter().GetResult();

            ArgumentNullException.ThrowIfNull(discovery);
            if (discovery.IsError) throw discovery.Exception!;
            ArgumentNullException.ThrowIfNull(discovery.JwksUri);

            var keyTask = Task.Run(async () => await http.GetStringAsync(discovery.JwksUri));
            var key = keyTask.GetAwaiter().GetResult();
            return new(key);
        }
    }

    /// <summary>
    /// Register the client.
    /// If the client is already registered, it will be ignored.
    /// </summary>
    /// <param name="target">The web application</param>
    /// <param name="clients">The clients to register</param>
    public static async Task RegisterClientsAsync(this WebApplication target, IEnumerable<IdentityClientRegistrarOptions> clients)
    {
        await using var scope = target.Services.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        foreach (var client in clients)
        {
            if (await manager.FindByClientIdAsync(client.ClientId) is not null)
            {
                continue;
            }

            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret,
                DisplayName = client.DisplayName,
            };
            var permissions = client.AllowScopes.Select(it => $"{Permissions.Prefixes.Scope}{it}")
                .Union(client.AllowEndpoints.Select(it => $"{Permissions.Prefixes.Endpoint}{it}"))
                .Union(client.AllowGrantTypes.Select(it => $"{Permissions.Prefixes.GrantType}{it}"))
                .Union(client.AllowResponseTypes.Select(it => $"{Permissions.Prefixes.ResponseType}{it}"));
            foreach (var item in permissions) descriptor.Permissions.Add(item);
            foreach (var item in client.Requirements) descriptor.Requirements.Add($"{Requirements.Prefixes.Feature}{item}");
            foreach (var item in client.LoginCallbackEndpoints) descriptor.RedirectUris.Add(new(item));
            foreach (var item in client.LogoutCallbackEndpoints) descriptor.PostLogoutRedirectUris.Add(new(item));
            await manager.CreateAsync(descriptor);
        }
    }

    /// <summary>
    /// Register the roles.
    /// If the role is already registered, it will be ignored.
    /// </summary>
    /// <param name="target">The web application</param>
    /// <param name="roles"></param>
    public static Task RegisterRolesAsync(this WebApplication target, IEnumerable<string> roles)
    {
        if (roles is null || false == roles.Any())
        {
            return Task.CompletedTask;
        }

        var uniqueRoleQry = roles
            .Where(it => false == string.IsNullOrWhiteSpace(it))
            .Distinct()
            .Select(it => new IdentityRole(it));
        return RegisterRolesAsync(target, uniqueRoleQry);
    }

    /// <summary>
    /// Register the roles.
    /// If the role is already registered, it will be ignored.
    /// </summary>
    /// <typeparam name="TIdentityRole">The type representing a Role in the system</typeparam>
    /// <param name="target">The web application</param>
    /// <param name="roles">The roles to register</param>
    public static async Task RegisterRolesAsync<TIdentityRole>(this WebApplication target, IEnumerable<TIdentityRole> roles)
        where TIdentityRole : IdentityRole
    {
        if (roles is null || false == roles.Any())
        {
            return;
        }

        var uniqueRoleQry = roles
            .Where(it => false == string.IsNullOrWhiteSpace(it.Name))
            .Distinct() ?? [];

        await using var scope = target.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TIdentityRole>>();
        foreach (var item in uniqueRoleQry)
        {
            if (await roleManager.FindByNameAsync(item.Name!) is not null)
            {
                continue;
            }

            await roleManager.CreateAsync(item);
        }
    }

    /// <summary>
    /// Register the accounts.
    /// If the account's email is already registered, it will be ignored.
    /// </summary>
    /// <typeparam name="TIdentityUser"></typeparam>
    /// <param name="target">The web application</param>
    /// <param name="accounts">The accounts to register</param>
    public static async Task RegisterAccountsAsync<TIdentityUser>(this WebApplication target, IEnumerable<RegisterAccount<TIdentityUser>> accounts)
        where TIdentityUser : IdentityUser
    {
        if (accounts is null || false == accounts.Any())
        {
            return;
        }

        var validAccountQry = accounts
            .Where(it => it is not null
                && it.User is not null
                && false == string.IsNullOrWhiteSpace(it.User.Email)
                && false == string.IsNullOrWhiteSpace(it.Password));

        await using var scope = target.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TIdentityUser>>();
        foreach (var item in validAccountQry)
        {
            var user = item.User!;
            if (await userManager.FindByEmailAsync(user.Email!) is not null)
            {
                continue;
            }

            await userManager.CreateAsync(user, item.Password);

            foreach (var role in item.Roles ?? [])
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}