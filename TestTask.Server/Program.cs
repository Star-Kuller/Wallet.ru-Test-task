using TestTask.Server.Controllers;
using TestTask.Server.Infrastructure;
using TestTask.Server.Infrastructure.Database;
using TestTask.Server.Interfaces;
using TestTask.Server.Interfaces.Database;
using TestTask.Server.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddTransient<IMessagesRepository, MessagesRepository>();
builder.Services.AddSingleton<IMessagesWebsocketController, MessagesWebsocketController>();
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddTransient<IDatabaseExecutor, DatabaseExecutor>();
builder.Services.AddSingleton<DatabaseParameters>(provider =>
    builder.Configuration.GetSection("Database").Get<DatabaseParameters>());

var app = builder.Build();

app.UseWebSockets();

app.Map("/wsMessages", async (HttpContext context, IMessagesWebsocketController messagesWsController) =>
{
    await messagesWsController.HandleWebSocket(context);
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseCors("AllowAll");

await CreateDatabaseAsync(app);

app.Run();

return;

async Task CreateDatabaseAsync(WebApplication webApplication)
{
    var databaseParameters = webApplication.Configuration.GetSection("Database").Get<DatabaseParameters>();
    var logger = webApplication.Services.GetRequiredService<ILogger<Program>>();
    var connectionFactory = webApplication.Services.GetRequiredService<IDbConnectionFactory>();
    await DatabaseInitialize.CreateDatabaseIfNotExist(databaseParameters, logger);
    await DatabaseInitialize.CreateTablesIfNotExist(connectionFactory, logger);
}