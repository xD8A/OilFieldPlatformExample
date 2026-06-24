namespace OilFieldPlatform.Calculation.Server.Schemas.Responses;

/// <summary>Уведомление об изменении пробы после расчёта эквивалентов.</summary>
public sealed class WaterSampleChangedResponse : IWaterSamplePageResponse
{
    /// <summary>Ключ изменённой пробы.</summary>
    public required string Key { get; init; }

    /// <summary>Флаг устаревания результата.</summary>
    public required bool IsOutdated { get; init; }

    /// <summary>Словарь эквивалентов: ключ — имя эквивалента (camelCase), значение — мг-экв/л или null.</summary>
    public required IDictionary<string, double?> Properties { get; init; }
}
