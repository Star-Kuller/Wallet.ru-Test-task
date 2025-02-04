using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TestTask.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var baseHost = builder.Configuration.GetValue<string>("Host")
               ?? throw new ArgumentException("Нет секции Host в appsettings.json");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseHost) });

await builder.Build().RunAsync();