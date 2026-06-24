using System.Text.Json.Serialization;
using OilFieldPlatform.Calculation.Core.Models;
using OilFieldPlatform.Domain.Enums;


namespace OilFieldPlatform.Calculation.Core.Proxies;

/// <summary>Прокси для пробы воды.</summary>
public sealed class WaterSampleProxyModel
{
    /// <summary>Конструктор прокси для пробы воды.</summary>
    /// <param name="sample">Проба воды.</param>
    public WaterSampleProxyModel(WaterSampleModel sample)
    {
        Sample = sample;
    }

    /// <summary>Построить ключ пробы по образцу.</summary>
    /// <param name="sample">Проба воды.</param>
    /// <returns>Ключ.</returns>
    public static string BuildKey(WaterSampleModel sample) => BuildKey(sample.SampledAt, sample.WaterType, sample.ClusterStationName, sample.WellName);
    /// <summary>Построить ключ пробы по компонентам.</summary>
    /// <param name="sampledAt">Дата отбора.</param>
    /// <param name="waterType">Тип воды.</param>
    /// <param name="clusterStationName">Наименование насосной станции.</param>
    /// <param name="wellName">Имя скважины.</param>
    /// <returns>Ключ.</returns>
    public static string BuildKey(DateTime sampledAt, WaterType waterType, string? clusterStationName, string? wellName) => $"{sampledAt:yyyy-MM-dd}:{waterType}:{clusterStationName ?? ""}:{wellName ?? ""}";

    /// <summary>Ключ пробы.</summary>
    public string Key => BuildKey(SampledAt, WaterType, ClusterStationName, WellName);

    /// <summary>Дата отбора пробы.</summary>
    public DateTime SampledAt => Sample.SampledAt;
    /// <summary>Тип воды (пластовая / закачиваемая).</summary>
    public WaterType WaterType => Sample.WaterType;
    /// <summary>Наименование насосной станции (КНС / ДНС).</summary>
    public string? ClusterStationName => Sample.ClusterStationName;
    /// <summary>Имя скважины.</summary>
    public string? WellName => Sample.WellName;

    /// <summary>Флаг устаревания результата.</summary>
    public bool IsOutdated { get => Sample.Equivalent.IsOutdated; set => Sample.Equivalent.IsOutdated = value; }

    /// <summary>Исходная проба воды.</summary>
    [JsonIgnore]
    public WaterSampleModel Sample { get; }

    // Ионы (мг/л)

    /// <summary>Cl⁻ (мг/л).</summary>
    public double? Chloride { get => Sample.Chloride; set => Sample.Chloride = value; }

    /// <summary>CO₃²⁻ (мг/л).</summary>
    public double? Carbonate { get => Sample.Carbonate; set => Sample.Carbonate = value; }

    /// <summary>HCO₃⁻ (мг/л).</summary>
    public double? Bicarbonate { get => Sample.Bicarbonate; set => Sample.Bicarbonate = value; }

    /// <summary>SO₄²⁻ (мг/л).</summary>
    public double? Sulfate { get => Sample.Sulfate; set => Sample.Sulfate = value; }

    /// <summary>Ca²⁺ (мг/л).</summary>
    public double? Calcium { get => Sample.Calcium; set => Sample.Calcium = value; }

    /// <summary>Mg²⁺ (мг/л).</summary>
    public double? Magnesium { get => Sample.Magnesium; set => Sample.Magnesium = value; }

    /// <summary>Na⁺ (мг/л).</summary>
    public double? Sodium { get => Sample.Sodium; set => Sample.Sodium = value; }

    /// <summary>Cl⁻ эквивалент (мг-экв/л).</summary>
    public double? ChlorideEquivalent => Sample.Equivalent.Chloride;

    /// <summary>CO₃²⁻ эквивалент (мг-экв/л).</summary>
    public double? CarbonateEquivalent => Sample.Equivalent.Carbonate;

    /// <summary>HCO₃⁻ эквивалент (мг-экв/л).</summary>
    public double? BicarbonateEquivalent => Sample.Equivalent.Bicarbonate;

    /// <summary>SO₄²⁻ эквивалент (мг-экв/л).</summary>
    public double? SulfateEquivalent => Sample.Equivalent.Sulfate;

    /// <summary>Ca²⁺ эквивалент (мг-экв/л).</summary>
    public double? CalciumEquivalent => Sample.Equivalent.Calcium;

    /// <summary>Mg²⁺ эквивалент (мг-экв/л).</summary>
    public double? MagnesiumEquivalent => Sample.Equivalent.Magnesium;

    /// <summary>Na⁺ эквивалент (мг-экв/л).</summary>
    public double? SodiumEquivalent => Sample.Equivalent.Sodium;

}
