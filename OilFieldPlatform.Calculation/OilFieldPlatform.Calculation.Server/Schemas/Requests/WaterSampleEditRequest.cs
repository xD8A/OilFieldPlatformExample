using System.Text.Json;

namespace OilFieldPlatform.Calculation.Server.Schemas.Requests;

/// <summary>Запрос редактирования ионного состава пробы.</summary>
public sealed class WaterSampleEditRequest : IWaterSamplePageRequest
{
    /// <summary>Тип сообщения.</summary>
    public string Type { get; init; } = "pages.waterSample.edit";

    /// <summary>Ключ пробы.</summary>
    public required string Key { get; init; }

    /// <summary>Имя редактируемого свойства (Chloride, Carbonate, …).</summary>
    public required string Property { get; init; }

    /// <summary>Новое значение свойства в JSON.</summary>
    public required JsonElement Value { get; init; }
}
