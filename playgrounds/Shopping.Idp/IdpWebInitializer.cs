﻿using Microsoft.EntityFrameworkCore;
using Shopping.Idp.DbContexts;
using System.Reflection;
using TTSS.Core.Web;
using TTSS.Core.Web.Identity.Server.Configurations;
using TTSS.Infra.Data.Sql;

namespace Shopping.Idp;

public class IdpWebInitializer : WebInitializerBase
{
    public override void RegisterOptions(IServiceCollection services)
    {
    }

    public override void RegisterServices(IServiceCollection services)
    {
        services
            .RegisterTTSSCoreHttp([Assembly.GetExecutingAssembly()])
            .RegisterIdentityServer<IdpDbContext>(new()
            {
                IsDevelopmentEnabled = true,
                Certificate = null, // THIS PROPERTY IS REQUIRED FOR PRODUCTION
            });
    }

    public override void RegisterDatabases(IServiceCollection services)
    {
        base.RegisterDatabases(services);

        var dbConnection = Configuration?.GetConnectionString("DefaultConnection");

        services
            .SetupSqlDatabase(it => it.UseNpgsql(dbConnection).UseOpenIddict())
                .AddDbContext<IdpDbContext>()
            .Build();
    }

    public override async Task PostBuildAsync(WebApplication app)
    {
        await base.PostBuildAsync(app);

        // DON'T DO THIS IN PRODUCTION! Just an example for demonstration purposes.
        var clientBaseUrl = "https://localhost:3001";
        var clients = new IdentityClientRegistrarOptions[]
        {
            new()
            {
                ClientId = "40D4C23A-0B90-4B1A-8D4E-4F0BE4E23D4B",
                DisplayName = "Shopping API",
                ClientSecret = "the$ecr3T",
                LoginCallbackEndpoints = [ IdentityClientRegistrarOptions.CreateLoginCallbackPath(clientBaseUrl)],
                LogoutCallbackEndpoints = [IdentityClientRegistrarOptions.CreateLogoutCallbackPath(clientBaseUrl)],
            }
        };
        await app.RegisterClientIfAbsentAsync(clients);
    }
}