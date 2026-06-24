using AutoMapper;
using NHibernate;
using NHibernate.Event;
using NLog.Extensions.Logging;
using OilFieldPlatform.Calculation.Core.Mapping;
using OilFieldPlatform.Calculation.Core.Services;
using OilFieldPlatform.Calculation.Core.States;
using OilFieldPlatform.Calculation.Server.Controllers;
using OilFieldPlatform.Calculation.Server.Services;
using OilFieldPlatform.Infrastructure.Mapping.Calculation;
using OilFieldPlatform.Infrastructure.Providers;
using OilFieldPlatform.Infrastructure.Repositories.Calculation;
using OilFieldPlatform.Infrastructure.Settings;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddNLog();

builder.Services.AddOpenApi();

// NHibernate (singleton — фабрики и настройки)
builder.Services.AddSingleton(sp =>
{
    var settings = new RedisSettings();
    sp.GetRequiredService<IConfiguration>().GetSection("Redis").Bind(settings);
    return settings;
});
builder.Services.AddSingleton(sp =>
{
    var settings = new DbSettings();
    sp.GetRequiredService<IConfiguration>().GetSection("Database").Bind(settings);
    return settings;
});
builder.Services.AddSingleton<UserNameProvider>();
builder.Services.AddSingleton<AuditableListener>();
builder.Services.AddSingleton<IPreInsertEventListener>(sp => sp.GetRequiredService<AuditableListener>());
builder.Services.AddSingleton<IPreUpdateEventListener>(sp => sp.GetRequiredService<AuditableListener>());
builder.Services.AddSingleton<DbListenerRegistry>();
builder.Services.AddSingleton<DbConfigProvider>();
builder.Services.AddSingleton(sp =>
{
    var configProvider = sp.GetRequiredService<DbConfigProvider>();
    var fluentConfig = configProvider.CreateFluentConfiguration(m =>
        m.FluentMappings.AddFromAssemblyOf<CalcProjectMap>());
    return fluentConfig.BuildSessionFactory();
});

// Redis
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<RedisSettings>();
    return ConnectionMultiplexer.Connect(settings.ConnectionString);
});
builder.Services.AddScoped<AppStateLoader>();

// Всё остальное — scoped (новый набор на каждое WebSocket-соединение)
builder.Services.AddScoped<NHibernate.ISession>(sp =>
    sp.GetRequiredService<ISessionFactory>().OpenSession());
builder.Services.AddScoped<CalcProjectReadRepository, CalcProjectRepository>();
builder.Services.AddScoped<CalcProjectRepository>();
builder.Services.AddSingleton<IMapper>(sp =>
{
    var loggerFactory = sp.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
    var logger = sp.GetRequiredService<ILogger<ProjectProfile>>();
    var config = new MapperConfiguration(cfg => cfg.AddProfile(new ProjectProfile(logger)), loggerFactory);
    config.AssertConfigurationIsValid();
    return config.CreateMapper();
});
builder.Services.AddScoped<ManageProjectService>();
builder.Services.AddScoped<ListProjectService>();
builder.Services.AddScoped<ApplicationState>();
builder.Services.AddScoped<ApplicationController>();
builder.Services.AddScoped<WaterSamplePageController>();
builder.Services.AddScoped<WebSocketService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
});

var staticFilesPath = app.Configuration.GetValue<string>("StaticFiles:RootPath") ?? "../OilFieldPlatform.Calculation.WebClient/dist";
var webClientPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, staticFilesPath));

var webClientProvider = Directory.Exists(webClientPath)
    ? new Microsoft.Extensions.FileProviders.PhysicalFileProvider(webClientPath)
    : null;

if (webClientProvider is not null)
{
    var staticFileOptions = new StaticFileOptions { FileProvider = webClientProvider };

    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = webClientProvider });
    app.UseStaticFiles(staticFileOptions);
    app.MapFallbackToFile("index.html", staticFileOptions);
}

app.Map("/ws", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var scope = context.RequestServices.CreateScope();
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        var webSocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
        var sessionId = context.Request.Query["sessionId"].FirstOrDefault();
        if (sessionId is not null)
            webSocketService.SessionId = sessionId;
        UserNameProvider.SetContextUser("test");
        await webSocketService.HandleAsync(ws, context.RequestAborted);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

await app.RunAsync();
