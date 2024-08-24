using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shopping.Shared;
using Shopping.Shared.DbContexts;
using Shopping.Shared.Interceptors;
using Shopping.WebApi.Biz.Tokens;
using System.Reflection;
using TTSS.Core;
using TTSS.Core.AspNetCore;
using TTSS.Core.AspNetCore.Pipelines;
using TTSS.Core.Messaging;
using TTSS.Core.Models;
using TTSS.Core.Services;
using TTSS.Infra.Data.Sql;

namespace Shopping.WebApi;

public sealed class PlaygroundWebInitializer : WebInitializerBase
{
    public override void RegisterOptions(IServiceCollection services)
    {
    }

    public override void RegisterServices(IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(DEMO_Context).Assembly,
            Assembly.GetExecutingAssembly(),
        };

        var pipelines = new[]
        {
            typeof(HttpUserIdentityPipelineBehavior<,>),
            typeof(HttpPagingPipelineValidator<,>),
        };

        services
            .RegisterWebModules()
            .RegisterCoreModules()
            .AddSingleton<IMappingStrategy, AutoMapperMappingStrategy>()
            .AddScoped<ICorrelationContext, DEMO_Context>()
            .AddAutoMapper(assemblies)
            .RegisterMessagingModule(assemblies, pipelines);

        // Optional for setting up Authentication.
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = CreateTokenHandler.Issuer,
                    ValidAudience = CreateTokenHandler.Audience,
                    IssuerSigningKey = CreateTokenHandler.SigningCreds.Key,
                };
            });

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

        var connBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
        var connection = new SqliteConnection(connBuilder.ConnectionString);

        services
            .SetupSqlDatabase(it => it.UseSqlite(connection, opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)))
                .AddDbContext<ShoppingDbContext>()
            .Build(cfg => cfg.Register<AuditInterceptor>());
    }
}
