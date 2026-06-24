using OilFieldPlatform.Calculation.Core.States.UI;

namespace OilFieldPlatform.Calculation.Server.Schemas.Responses;

/// <summary>Ответ с полным состоянием страницы проб воды.</summary>
public sealed class WaterSampleStateResponse : IWaterSamplePageResponse
{
    /// <summary>Состояние страницы проб воды.</summary>
    public required WaterSamplePageState State { get; init; }
}
