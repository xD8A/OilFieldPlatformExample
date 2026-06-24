namespace OilFieldPlatform.Calculation.Server.Schemas.Responses;

/// <summary>Ответ на редактирование ионного состава пробы.</summary>
public sealed class WaterSampleEditedResponse : IWaterSamplePageResponse
{
    /// <summary>Ключ отредактированной пробы.</summary>
    public required string Key { get; init; }

    /// <summary>Имя изменённого свойства.</summary>
    public required string Property { get; init; }
}
