using System.Reflection;
using TTSS.Core.Web;

namespace Sample14.Basic.WebApp.WebInitializer;

public class DemoWebInitializer : WebInitializerBase
{
    public override void RegisterServices(IServiceCollection services)
    {
        services
            .RegisterTTSSCoreHttp([Assembly.GetExecutingAssembly()]);
    }
}