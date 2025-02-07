using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TestTask.Client;
using TestTask.Client.Interfaces;
using TestTask.Client.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.ConfigureRootComponents();

var serverAdders = builder.Configuration.GetServerUriAddress();
var serverWsAddress = builder.Configuration.GetServerWebSocketAddress("wsMessages");

builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddSingleton<ICounterService, CounterService>();

builder.Services.AddScoped(
    _ => new HttpClient { BaseAddress = serverAdders });
builder.Services.AddTransient<IWebSocketService<Message>>(
    _ => new WebSocketService<Message>(serverWsAddress));

await builder.Build().RunAsync();
