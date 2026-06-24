using OilFieldPlatform.Calculation.Server.Schemas;
using OilFieldPlatform.Calculation.Server.Schemas.Responses;

namespace OilFieldPlatform.Calculation.Server.Controllers;

/// <summary>Интерфейс контроллера, обрабатывающего WebSocket-запросы.</summary>
public interface IWebSocketController
{
    /// <summary>Обработать запрос.</summary>
    /// <param name="request">Входящий запрос.</param>
    /// <returns>Ответ или null, если запрос не относится к данному контроллеру.</returns>
    IWebSocketResponse? HandleRequest(IWebSocketRequest request);

    /// <summary>Опубликовать лог-сообщение, если подключение активно.</summary>
    /// <param name="response">Лог-сообщение.</param>
    void PublishLog(LogResponse response);

    /// <summary>Событие изменения состояния (уведомления, не привязанные к конкретному запросу).</summary>
    event EventHandler<IWebSocketResponse>? OnChanged;
}
