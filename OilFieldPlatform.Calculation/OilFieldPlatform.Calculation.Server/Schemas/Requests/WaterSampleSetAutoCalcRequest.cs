namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос установки режима автоматического расчёта.</summary>
public sealed class WaterSampleSetAutoCalcRequest : IWaterSamplePageRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "pages.waterSample.setAutoCalc";

    /// <summary>Флаг авто-расчёта.</summary>
    public required bool IsAuto { get; init; }
}
