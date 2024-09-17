using System.Reflection;
using TTSS.Core.Web;

var builder = WebApplication.CreateBuilder(args);

// Register TTSS Core services
builder.Services
    .RegisterTTSSCoreHttp([Assembly.GetExecutingAssembly()]);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Key takeaways from this example:
// The project structure is like a Console App, but we removed the Calculatormessage and handler since users can call APIs directly.
// Instead, we use handlers directly in the controller.