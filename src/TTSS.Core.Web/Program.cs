using TTSS.Core.Configurations;
using TTSS.Core.Web;

var builder = WebApplication.CreateBuilder(args);
builder.AddOptionsValidator<Opt>();

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
    }
}

file class Opt : IOptionsValidator
{
    public static string SectionName => nameof(Opt);

    public int Age { get; set; }

    public bool Validate()
        => Age > 5;
}