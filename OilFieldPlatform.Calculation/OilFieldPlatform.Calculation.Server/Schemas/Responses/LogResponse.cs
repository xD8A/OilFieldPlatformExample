namespace OilFieldPlatform.Calculation.Server.Schemas.Responses;

/// <summary>Уведомление с лог-сообщением от сервера.</summary>
public sealed class LogResponse : IWaterSamplePageResponse
{
    /// <summary>Уровень лога (Information, Warning, Error, Critical).</summary>
    public required string Level { get; init; }

    /// <summary>Текст сообщения.</summary>
    public required string Message { get; init; }
}
