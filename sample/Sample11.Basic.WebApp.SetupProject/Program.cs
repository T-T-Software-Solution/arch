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
// 1. We use TTSS.Core.Web instead of TTSS.Core.
// 2. TTSS.Core is registered in ASP.NET with the RegisterTTSSCoreHttp method, similar to a Console App but replacing RegisterTTSSCore.
// 3. MessagingHub is resolved from DI in any Controller.
// 4. Message models should implement IHttpRequesting instead of IRequesting.
// 5. Message handlers should inherit from HttpRequestHandler<TRequest> instead of RequestHandler<TRequest>.
// 6. Message handlers should return IHttpResponse.
// 7. We use ApiControllerBase instead of ControllerBase, and controller names must be plural.
// 8. TTSS.Core.Web includes liveness endpoints by default.