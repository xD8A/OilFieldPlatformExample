namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос состояния страницы проб воды.</summary>
public sealed class WaterSampleGetStateRequest : IWaterSamplePageRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "pages.waterSample.getState";
}
