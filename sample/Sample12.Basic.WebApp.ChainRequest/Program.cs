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
// 1. API handlers must inherit from HttpRequestHandler<TRequest> for HTTP responses.
// 2. Internal handlers can inherit from RequestHandler<TRequest>.
// 3. Internal handlers and API handlers can interact normally.
// 4. Message models can be used as API payloads.