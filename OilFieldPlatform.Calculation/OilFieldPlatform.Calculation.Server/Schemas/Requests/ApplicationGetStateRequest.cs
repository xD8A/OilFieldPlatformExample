namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос состояния приложения.</summary>
public sealed class ApplicationGetStateRequest : IApplicationRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "application.getState";
}
