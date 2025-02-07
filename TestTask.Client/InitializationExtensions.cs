using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace TestTask.Client;

public static class InitializationExtensions
{
    public static void ConfigureRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
    }

    public static Uri GetServerUriAddress(this IConfiguration configuration)
    {
        var serverHost = configuration.GetValue<string>("ServerHost")
                         ?? throw new ArgumentException("Нет секции ServerHost в appsettings.json");

        var useHttps = configuration.GetValue<bool>("UseHttps");
        var protocol = useHttps ? "https" : "http";
        var hostWithProtocol = $"{protocol}://{serverHost}";

        return new Uri(hostWithProtocol);
    }
    
    public static Uri GetServerWebSocketAddress(this IConfiguration configuration, string path)
    {
        var serverHost = configuration.GetValue<string>("ServerHost")
                         ?? throw new ArgumentException("Нет секции ServerHost в appsettings.json");
        var useHttps = configuration.GetValue<bool>("UseHttps");
        var protocol = useHttps ? "wss" : "ws";
        var hostWithProtocol = $"{protocol}://{serverHost}/{path}";
        
        return new Uri(hostWithProtocol);
    }
}