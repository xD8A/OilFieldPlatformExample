namespace OilFieldPlatform.Calculation.Server.Schemas.Responses;

/// <summary>Ответ на установку режима авто-расчёта.</summary>
public sealed class WaterSampleAutoCalcSetResponse : IWaterSamplePageResponse
{
    /// <summary>Установленное значение флага авто-расчёта.</summary>
    public required bool IsAuto { get; init; }
}
