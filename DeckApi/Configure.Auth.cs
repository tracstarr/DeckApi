using DeckApi.ServiceInterface.Data;
using ServiceStack.Auth;

[assembly: HostingStartup(typeof(ConfigureAuth))]

namespace DeckApi;

public class ConfigureAuth : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services =>
        {
            services.AddPlugin(new AuthFeature(IdentityAuth.For<ApplicationUser>(options => {
                options.SessionFactory = () => new AuthUserSession();
                options.CredentialsAuth();
              
            })));
        });
       
}