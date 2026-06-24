namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос подключения к реактивному списку проб.</summary>
public sealed class WaterSampleConnectRequest : IWaterSamplePageRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "pages.waterSample.connect";
}
