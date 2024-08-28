using Shopping.Idp;
using System.Reflection;
using TTSS.Core.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

await builder.RunAsync<IdpWebInitializer>(app =>
{
    if (app.Environment.IsProduction())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapRazorPages();
    app.MapGet("/", () => $"{Assembly.GetExecutingAssembly().GetName().Name} (Mode: {app.Environment.EnvironmentName})");
});