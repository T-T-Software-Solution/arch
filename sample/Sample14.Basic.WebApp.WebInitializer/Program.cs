using Sample14.Basic.WebApp.WebInitializer;
using TTSS.Core.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

await builder.RunAsync<DemoWebInitializer>(app =>
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
});

// Key takeaways from this example:
// 1. We moved the initialization logic to a separate class (DemoWebInitializer) to keep Program.cs clean.
// 2. The initialization logic can now be shared across multiple web projects with the same setup.