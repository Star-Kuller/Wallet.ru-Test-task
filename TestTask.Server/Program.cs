using Npgsql;
using TestTask.Server.Controllers;
using TestTask.Server.Infrastructure;
using TestTask.Server.Interfaces;
using TestTask.Server.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
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
builder.Services.AddSingleton<DatabaseParameters>(provider =>
    builder.Configuration.GetSection("Database").Get<DatabaseParameters>());

var app = builder.Build();

app.UseWebSockets();

app.Map("/wsMessages", async (HttpContext context, IMessagesWebsocketController messagesWsController) =>
{
    await messagesWsController.HandleWebSocket(context);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseCors("AllowAll");

var databaseParameters = app.Configuration.GetSection("Database").Get<DatabaseParameters>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var connectionFactory = app.Services.GetRequiredService<IDbConnectionFactory>();
await DatabaseInitialize.CreateDatabaseIfNotExist(databaseParameters, logger);
await DatabaseInitialize.CreateTablesIfNotExist(connectionFactory, logger);

app.Run();