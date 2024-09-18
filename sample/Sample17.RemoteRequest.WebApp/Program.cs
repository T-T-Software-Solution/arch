using Sample17.RemoteRequest.WebApp;
using TTSS.Core.Web;

// INSTRUCTIONS:
// 1. Start the PostgreSQL server. (Update PostgreSQL connection string in appsettings.json if needed)
// 2. Run Sample16.RemoteRequest.ConsoleApp1.
// 3. Run Sample17.RemoteRequest.WebApp.
// 4. Watch the console output.
// 5. Run Swagger UI and call the get Pings endpoint.
// 6. Run Sample16.RemoteRequest.ConsoleApp2.
// 7. Watch the console output.
// 8. Run Swagger UI and call the post Dogs endpoint.
// 9. Check the database for the new record.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

await builder.RunAsync<MyWebInitializer>(app =>
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
// 1. Different project types can communicate as if they're in the same app.
// 2. Use typeof(T).Assembly to reference other project assemblies.
// 3. Messages are sent to a randomly selected handler to balance the workload.
// 4. A single database stores messages and animals in separate schemas.