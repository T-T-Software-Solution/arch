using Sample15.Basic.WebApp.WorkWithDatabase;
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
// 1. Database use is similar to the Console App, but DbContext registration is moved to WebInitializer.
// 2. Model mapping to view models uses IMapper, just like in the Console App example.