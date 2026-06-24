using OilFieldPlatform.Domain.Entities.Calculation.Data;
using FluentNHibernate.Mapping;

namespace OilFieldPlatform.Infrastructure.Mapping.Calculation.Data;

/// <summary>
/// NHibernate-маппинг сущности CalcWaterSampleEquivalentRecord (эквиваленты ионов).
/// PK = FK на CalcWaterSampleEntity (sample_id).
/// </summary>
public sealed class CalcWaterSampleEquivalentMap : ClassMap<CalcWaterSampleEquivalentRecord>
{
    /// <summary>
    /// Настройка маппинга: таблица calc_water_sample_equivalents, one-to-one PK=FK.
    /// </summary>
    public CalcWaterSampleEquivalentMap()
    {
        Table("calc_water_sample_equivalents");
        Id(x => x.Id).Column("sample_id")                                                      // PK + FK → calc_water_samples
            .GeneratedBy.Foreign("WaterSample");                                                // PK генерируется из FK (one-to-one)
        HasOne(x => x.WaterSample).Constrained();                                               // Связь с пробой (one-to-one)
        Map(x => x.Chloride).Column("chloride");                                                // Cl⁻ (мг-экв/л)
        Map(x => x.Carbonate).Column("carbonate");                                              // CO₃²⁻ (мг-экв/л)
        Map(x => x.Bicarbonate).Column("bicarbonate");                                          // HCO₃⁻ (мг-экв/л)
        Map(x => x.Sulfate).Column("sulfate");                                                  // SO₄²⁻ (мг-экв/л)
        Map(x => x.Calcium).Column("calcium");                                                  // Ca²⁺ (мг-экв/л)
        Map(x => x.Magnesium).Column("magnesium");                                              // Mg²⁺ (мг-экв/л)
        Map(x => x.Sodium).Column("sodium");                                                    // Na⁺ (мг-экв/л)
        Map(x => x.CreatedAt).Column("created_at");                                             // Дата создания записи
        Map(x => x.UpdatedAt).Column("updated_at");                                             // Дата последнего обновления записи
        Map(x => x.CreatedBy).Column("created_by");                                             // Автор создания
        Map(x => x.UpdatedBy).Column("updated_by");                                             // Автор последнего изменения
    }
}
