namespace OilFieldPlatform.Calculation.Server.Schemas;

/// <summary>Базовый интерфейс для всех входящих WebSocket-сообщений (запросов).</summary>
public interface IWebSocketRequest
{
    /// <summary>Тип сообщения.</summary>
    string Type { get; }
}
