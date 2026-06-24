#pragma warning disable S1210, CA1036 // CompareTo used only for sorting, no operator overrides needed
using OilFieldPlatform.Domain.Entities.ABC;
using OilFieldPlatform.Domain.Entities.Calculation.Data;
using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Domain.Enums;
using OilFieldPlatform.Domain.Interfaces;

namespace OilFieldPlatform.Domain.Entities.Calculation;

/// <summary>
/// Проба воды (расчётный проект).
/// </summary>
public class CalcWaterSampleEntity : ABCEntity<CalcWaterSampleEntity>, IAnionSample, ICationSample, IComparable<CalcWaterSampleEntity>
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="project">Расчётный проект, к которому относится проба.</param>
    /// <param name="sampledAt">Дата и время отбора пробы.</param>
    /// <param name="waterType">Тип воды (Reservoir / Injection).</param>
    /// <param name="clusterStationName">Номер / наименование скважины.</param>
    /// <param name="wellName">Наименование насосной станции (КНС / ДНС).</param>
    /// <param name="id">Идентификатор (опционально).</param>
    public CalcWaterSampleEntity(CalcProjectEntity project, DateTime sampledAt, WaterType waterType, string? clusterStationName, string? wellName, long? id = null) : base(id)
    {
        Project = project;
        SampledAt = sampledAt;
        ClusterStationName = clusterStationName;
        WellName = wellName;
        WaterType = waterType;
    }

    /// <summary>
    /// Конструктор для NHibernate.
    /// </summary>
    protected CalcWaterSampleEntity() : base()
    {
        Project = null!;
    }

    /// <summary>
    /// Расчётный проект.
    /// </summary>
    public virtual CalcProjectEntity Project { get; set; }

    /// <summary>
    /// Исходная проба воды (из БД).
    /// </summary>
    public virtual WaterSampleEntity? SourceSample { get; set; }

    /// <summary>
    /// Имя скважины, из которой отобрана проба.
    /// </summary>
    public virtual string? WellName { get; set; }

    /// <summary>
    /// Наименование насосной станции (КНС / ДНС), с которой связана проба.
    /// </summary>
    public virtual string? ClusterStationName { get; set; }

    /// <summary>
    /// Дата отбора пробы.
    /// </summary>
    public virtual DateTime SampledAt { get; set; }

    /// <summary>
    /// Тип воды (пластовая / закачиваемая).
    /// </summary>
    public virtual WaterType WaterType { get; set; }

    /// <summary>
    /// Cl⁻ (мг/л).
    /// </summary>
    public virtual double? Chloride { get; set; }

    /// <summary>
    /// CO₃²⁻ (мг/л).
    /// </summary>
    public virtual double? Carbonate { get; set; }

    /// <summary>
    /// HCO₃⁻ (мг/л).
    /// </summary>
    public virtual double? Bicarbonate { get; set; }

    /// <summary>
    /// Ca²⁺ (мг/л).
    /// </summary>
    public virtual double? Calcium { get; set; }

    /// <summary>
    /// Mg²⁺ (мг/л).
    /// </summary>
    public virtual double? Magnesium { get; set; }

    /// <summary>
    /// Na⁺ (мг/л).
    /// </summary>
    public virtual double? Sodium { get; set; }

    /// <summary>
    /// SO₄²⁻ (мг/л).
    /// </summary>
    public virtual double? Sulfate { get; set; }

    /// <summary>
    /// Расчётные эквиваленты ионов.
    /// </summary>
    public virtual CalcWaterSampleEquivalentRecord? Equivalent { get; set; }

    /// <summary>
    /// Сравнение по дате отбора, типу воды, насосной станции, скважине.
    /// </summary>
    /// <param name="other">Другая проба для сравнения.</param>
    /// <returns>Результат сравнения: -1, 0 или 1.</returns>
    public virtual int CompareTo(CalcWaterSampleEntity? other)
    {
        if (other is null) return 1;

        var dateComparison = SampledAt.CompareTo(other.SampledAt);
        if (dateComparison != 0) return dateComparison;

        var waterTypeComparison = WaterType.CompareTo(other.WaterType);
        if (waterTypeComparison != 0) return waterTypeComparison;

        var stationComparison = string.Compare(ClusterStationName, other.ClusterStationName, StringComparison.Ordinal);
        if (stationComparison != 0) return stationComparison;

        var wellComparison = string.Compare(WellName, other.WellName, StringComparison.Ordinal);
        return wellComparison;
    }
}
