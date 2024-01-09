using TTSS.Core.AspNetCore;
using TTSS.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.AddOptionsValidator<Opt>();

builder.Services.AddControllers();
var app = await builder.BuildAsync<WebInitializer>();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


file class WebInitializer : WebInitializerBase
{
    public override void RegisterOptions(IServiceCollection services)
    {
    }

    public override void RegisterServices(IServiceCollection services)
    {
    }
}

file class Opt : IOptionsValidator
{
    public static string SectionName => nameof(Opt);

    public int Age { get; set; }

    public bool Validate()
        => Age > 5;
}