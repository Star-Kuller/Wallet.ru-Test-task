using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TestTask.Client;
using TestTask.Client.Interfaces;
using TestTask.Client.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var serverHost = builder.Configuration.GetValue<string>("ServerHost")
               ?? throw new ArgumentException("Нет секции ServerHost в appsettings.json");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serverHost) });
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddSingleton<ICounterService, CounterService>();

await builder.Build().RunAsync();