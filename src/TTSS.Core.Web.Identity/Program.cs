using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TTSS.Core.Web;
using TTSS.Core.Web.Identity.Server.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var application = await builder.BuildAsync<WebInitializer>(app =>
{
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
});

application.Run();

file class WebInitializer : WebInitializerBase
{
    public override void RegisterOptions(IServiceCollection services)
    {
    }

    public override void RegisterServices(IServiceCollection services)
    {
        services
            .RegisterTTSSCoreHttp([Assembly.GetExecutingAssembly()])
            .RegisterIdentityServer<SampleIdentityDbContext>(new()
            {
                IsDevelopmentEnabled = true,
                IdentityBuilder = builder =>
                {
                    builder.AddDefaultUI();
                }
            });
    }

    public override void RegisterDatabases(IServiceCollection services)
    {
        base.RegisterDatabases(services);
        // Ensure the database is registered with a DbContext that inherits from IdentityDbContextBase.
    }
}

file class SampleIdentityDbContext(DbContextOptions options) : IdentityDbContextBase(options);