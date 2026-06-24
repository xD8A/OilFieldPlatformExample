namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос отключения от реактивного списка проб.</summary>
public sealed class WaterSampleDisconnectRequest : IWaterSamplePageRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "pages.waterSample.disconnect";
}
