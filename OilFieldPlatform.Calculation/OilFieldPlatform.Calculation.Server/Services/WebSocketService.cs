using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using OilFieldPlatform.Calculation.Server.Controllers;
using OilFieldPlatform.Calculation.Server.Schemas;
using OilFieldPlatform.Calculation.Server.Schemas.Requests;


namespace OilFieldPlatform.Calculation.Server.Services;

/// <summary>Сервис WebSocket — владеет контроллерами и обрабатывает сообщения.</summary>
public sealed class WebSocketService : IDisposable
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger _logger;
    private readonly AppStateLoader _stateLoader;
    private bool _disposed;

    /// <summary>Идентификатор сессии (из query-параметра или случайный).</summary>
    public string SessionId { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>Контроллер приложения.</summary>
    public ApplicationController Application { get; }

    /// <summary>Контроллер страницы проб воды.</summary>
    public WaterSamplePageController WaterSamplePage { get; }

    /// <summary>Конструктор.</summary>
    public WebSocketService(
        ApplicationController application,
        WaterSamplePageController waterSamplePage,
        AppStateLoader stateLoader,
        ILogger<WebSocketService>? logger = null)
    {
        _logger = (ILogger?)logger ?? NullLogger.Instance;
        _stateLoader = stateLoader;

        Application = application;
        WaterSamplePage = waterSamplePage;

        Application.OnChanged += ChangedHandler;
        WaterSamplePage.OnChanged += ChangedHandler;
    }

    private async void ChangedHandler(object? sender, IWebSocketResponse response)
    {
        await SendAsync(JsonSerializer.Serialize(response, _jsonOptions), CancellationToken.None);
    }

    private WebSocket? _currentSocket;

    /// <summary>Обработать WebSocket-соединение.</summary>
    /// <param name="webSocket">WebSocket.</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task HandleAsync(WebSocket webSocket, CancellationToken ct = default)
    {
        _currentSocket = webSocket;
        _logger.LogDebug("WebSocket connection opened, session={SessionId}", SessionId);

        await SendAsync(JsonSerializer.Serialize(
            new { type = "session.info", sessionId = SessionId }, _jsonOptions), ct);

        await RestoreStateAsync();

        // Автосохранение при изменении проекта или его данных
        var appState = Application.GetApplicationState();
        var saveSub = appState.ProjectAsObservable
            .Select(p => p?.IsChangedAsObservable ?? Observable.Return(false))
            .Switch()
            .Throttle(TimeSpan.FromSeconds(5))
            .Subscribe(async _ =>
            {
                try { await SaveStateAsync(); }
                catch (Exception ex) { _logger.LogWarning(ex, "Auto-save failed"); }
            });

        var buffer = new byte[1024 * 16];
        var lastSave = DateTime.UtcNow;

        while (webSocket.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            var saveDelay = TimeSpan.FromMinutes(1) - (DateTime.UtcNow - lastSave);
            var delayTask = Task.Delay(saveDelay > TimeSpan.Zero ? saveDelay : TimeSpan.Zero, ct);
            var receiveTask = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

            if (await Task.WhenAny(receiveTask, delayTask) == delayTask)
            {
                await SaveStateAsync();
                lastSave = DateTime.UtcNow;
                continue;
            }

            WebSocketReceiveResult result;
            try
            {
                result = await receiveTask;
            }
            catch (WebSocketException ex)
            {
                _logger.LogWarning(ex, "WebSocket receive error");
                break;
            }

            if (result.MessageType == WebSocketMessageType.Close)
            {
                _logger.LogDebug("WebSocket close received");
                try
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "WebSocket close error (client may have disconnected)");
                }
                break;
            }

            if (result.MessageType == WebSocketMessageType.Text && result.Count > 0)
            {
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await ProcessMessageAsync(json, ct);
            }
        }

        await SaveStateAsync();
        saveSub.Dispose();

        _currentSocket = null;
        _logger.LogDebug("WebSocket connection closed, session={SessionId}", SessionId);
    }

    private async Task RestoreStateAsync()
    {
        var state = await _stateLoader.LoadAsync(SessionId);
        if (state is not null)
        {
            Application.RestoreState(state);
            _logger.LogInformation("Session {SessionId}: state restored", SessionId);
        }
    }

    private async Task SaveStateAsync()
    {
        var state = Application.GetApplicationState();
        await _stateLoader.SaveAsync(SessionId, state);
        _logger.LogDebug("Session {SessionId}: state saved", SessionId);
    }

    private async Task ProcessMessageAsync(string json, CancellationToken ct)
    {
        try
        {
            var request = DeserializeRequest(json);
            if (request is null)
            {
                _logger.LogWarning("WebSocket message missing or unknown 'type' field");
                return;
            }

            _logger.LogDebug("WebSocket message received: {Type}", request.Type);

            IWebSocketResponse? response = request switch
            {
                IApplicationRequest r => Application.HandleRequest(r),
                IWaterSamplePageRequest r => WaterSamplePage.HandleRequest(r),
                _ => null
            };

            if (response is not null)
            {
                var message = JsonSerializer.Serialize(response, _jsonOptions);
                await SendAsync(message, ct);
            }
            else
                _logger.LogWarning("Unhandled message type: {Type}", request.Type);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid JSON in WebSocket message");
        }
    }

    private static IWebSocketRequest? DeserializeRequest(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var type = doc.RootElement.TryGetProperty("type", out var typeProp) ? typeProp.GetString() : null;

        return type switch
        {
            "application.listProjects" => JsonSerializer.Deserialize<ApplicationListProjectRequest>(json, _jsonOptions),
            "application.getState" => JsonSerializer.Deserialize<ApplicationGetStateRequest>(json, _jsonOptions),
            "application.openProject" => JsonSerializer.Deserialize<ApplicationOpenProjectRequest>(json, _jsonOptions),
            "application.saveProject" => JsonSerializer.Deserialize<ApplicationSaveProjectRequest>(json, _jsonOptions),
            "application.closeProject" => JsonSerializer.Deserialize<ApplicationCloseProjectRequest>(json, _jsonOptions),
            "pages.waterSample.getState" => JsonSerializer.Deserialize<WaterSampleGetStateRequest>(json, _jsonOptions),
            "pages.waterSample.connect" => JsonSerializer.Deserialize<WaterSampleConnectRequest>(json, _jsonOptions),
            "pages.waterSample.disconnect" => JsonSerializer.Deserialize<WaterSampleDisconnectRequest>(json, _jsonOptions),
            "pages.waterSample.edit" => JsonSerializer.Deserialize<WaterSampleEditRequest>(json, _jsonOptions),
            "pages.waterSample.setAutoCalc" => JsonSerializer.Deserialize<WaterSampleSetAutoCalcRequest>(json, _jsonOptions),
            "pages.waterSample.calculate" => JsonSerializer.Deserialize<WaterSampleCalculateRequest>(json, _jsonOptions),
            _ => null
        };
    }

    private async Task SendAsync(string json, CancellationToken ct)
    {
        if (_currentSocket?.State != WebSocketState.Open)
            return;

        var bytes = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(bytes);

        try
        {
            await _currentSocket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }
        catch (WebSocketException ex)
        {
            _logger.LogWarning(ex, "WebSocket send error");
        }
    }

    /// <summary>Освобождение ресурсов.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        Application.OnChanged -= ChangedHandler;
        WaterSamplePage.OnChanged -= ChangedHandler;
        WaterSamplePage.Dispose();
        (Application as IDisposable)?.Dispose();
        _disposed = true;
    }
}
