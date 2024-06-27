using Funq;
using DeckApi.ServiceInterface;
using ServiceStack.DataAnnotations;

[assembly: HostingStartup(typeof(AppHost))]

namespace DeckApi;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            // Configure ASP.NET Core IOC Dependencies
        });

    public AppHost() : base("DeckApi", typeof(CartService).Assembly) {}

    public override void Configure(Container container)
    {
        // Configure ServiceStack only IOC, Config & Plugins
        SetConfig(new HostConfig {
            UseSameSiteCookies = true,
            Return204NoContentForEmptyResponse = false
        });
        
        ConfigurePlugin<PredefinedRoutesFeature>(feature => feature.JsonApiRoute = null);
        //typeof(Authenticate).AddAttributes(new ExcludeAttribute(Feature.Metadata));  
    }
}
