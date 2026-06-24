namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос пересчёта эквивалентов всех проб.</summary>
public sealed class WaterSampleCalculateRequest : IWaterSamplePageRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "pages.waterSample.calculate";
}
