#pragma warning disable CA1036 // CompareTo delegated to WaterSample, no operator overrides needed
using OilFieldPlatform.Domain.Interfaces;
using System.Diagnostics;

namespace OilFieldPlatform.Domain.Entities.Calculation.Data;

/// <summary>
/// Запись эквивалентов ионов для пробы воды в расчётном проекте (результат расчёта).
/// Привязана к CalcWaterSampleEntity через первичный ключ (PK = FK).
/// </summary>
public class CalcWaterSampleEquivalentRecord : IRecord<CalcWaterSampleEquivalentRecord>, IAnionSample, ICationSample
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="waterSample">Проба воды, к которой относится запись эквивалентов (одновременно PK и FK).</param>
    public CalcWaterSampleEquivalentRecord(CalcWaterSampleEntity waterSample)
    {
        WaterSample = waterSample;
    }

    /// <summary>
    /// Конструктор для NHibernate.
    /// </summary>
    protected CalcWaterSampleEquivalentRecord()
    {
        WaterSample = null!;
    }

    /// <summary>
    /// Идентификатор записи (PK, совпадает с Id пробы).
    /// </summary>
    public virtual long Id { get; set; }

    /// <summary>
    /// Проба воды (первичный ключ + внешний ключ).
    /// </summary>
    public virtual CalcWaterSampleEntity WaterSample { get; set; }

    /// <summary>
    /// Cl⁻ (мг-экв/л).
    /// </summary>
    public virtual double? Chloride { get; set; }

    /// <summary>
    /// CO₃²⁻ (мг-экв/л).
    /// </summary>
    public virtual double? Carbonate { get; set; }

    /// <summary>
    /// HCO₃⁻ (мг-экв/л).
    /// </summary>
    public virtual double? Bicarbonate { get; set; }

    /// <summary>
    /// SO₄²⁻ (мг-экв/л).
    /// </summary>
    public virtual double? Sulfate { get; set; }

    /// <summary>
    /// Ca²⁺ (мг-экв/л).
    /// </summary>
    public virtual double? Calcium { get; set; }

    /// <summary>
    /// Mg²⁺ (мг-экв/л).
    /// </summary>
    public virtual double? Magnesium { get; set; }

    /// <summary>
    /// Na⁺ (мг-экв/л).
    /// </summary>
    public virtual double? Sodium { get; set; }

    /// <summary>
    /// Дата создания записи.
    /// </summary>
    public virtual DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления записи.
    /// </summary>
    public virtual DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Кем создана запись.
    /// </summary>
    public virtual string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Кем последний раз обновлена запись.
    /// </summary>
    public virtual string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Сравнение с другой записью через привязанную пробу.
    /// </summary>
    /// <param name="other">Другая запись для сравнения.</param>
    /// <returns>Результат сравнения привязанных проб (-1, 0 или 1).</returns>
    public virtual int CompareTo(CalcWaterSampleEquivalentRecord? other)
    {
        if (other is null) return 1;
        Debug.Assert(WaterSample is not null, "WaterSample should not be null when comparing CalcWaterSampleEquivalentRecord");
        return WaterSample.CompareTo(other.WaterSample);
    }
}
