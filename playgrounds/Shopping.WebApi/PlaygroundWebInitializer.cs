using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shopping.Shared;
using Shopping.Shared.DbContexts;
using Shopping.Shared.Interceptors;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.Web;
using TTSS.Infra.Data.Sql;

namespace Shopping.WebApi;

public sealed class PlaygroundWebInitializer : WebInitializerBase
{
    private string DbConnectionString => Configuration?.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string is not found.");

    public override void RegisterServices(IServiceCollection services)
    {
        var assemblies = new[]
        {
            Assembly.GetExecutingAssembly(),
            typeof(MappingProfileRegistrar).Assembly,
        };

        services
           .AddOptions<SqlTransportOptions>()
           .Configure(option => option.ConnectionString = DbConnectionString);

        services
            .RegisterTTSSCoreHttp(assemblies)
            .RegisterIdentityClient<ShoppingDbContext>(new()
            {
                IsDevelopmentEnabled = true,
                CredentialKey = null, // THIS PROPERTY IS REQUIRED FOR PRODUCTION
                AuthorityBaseUrl = "https://localhost:9001/",
                RequiredAudienceBaseUrls = ["https://localhost:3001/"],
                ClientId = "40D4C23A-0B90-4B1A-8D4E-4F0BE4E23D4B",
                ClientSecret = "the$ecr3T",
                ProviderName = "shopping-idp",
            })
            .AddPostgresMigrationHostedService()
            .RegisterRemoteRequest(assemblies, (bus, configure) => bus.UsingPostgres(configure));

        // Optional for setting up Swagger's authentication.
        services
           .AddSwaggerGen(cfg =>
           {
               const string Bearer = nameof(Bearer);
               cfg.AddSecurityDefinition(Bearer, new()
               {
                   Scheme = Bearer,
                   BearerFormat = "JWT",
                   Name = "Authorization",
                   In = ParameterLocation.Header,
                   Type = SecuritySchemeType.Http,
               });

               var bearerRef = new OpenApiReference { Id = Bearer, Type = ReferenceType.SecurityScheme, };
               cfg.AddSecurityRequirement(new()
               {
                   { new (){ Reference = bearerRef}, [] }
               });
           });
    }

    public override void RegisterDatabases(IServiceCollection services)
    {
        base.RegisterDatabases(services);

        services
            .SetupSqlDatabase(it => it.UseNpgsql(DbConnectionString).UseOpenIddict())
                .AddDbContext<ShoppingDbContext>()
            .Build(cfg => cfg.Register<AuditInterceptor>());
    }
}