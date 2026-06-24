using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Domain.Enums;
using FluentNHibernate.Mapping;

namespace OilFieldPlatform.Infrastructure.Mapping.Common;

/// <summary>
/// NHibernate-маппинг сущности WaterSampleEntity (проба воды).
/// </summary>
public sealed class WaterSampleMap : ClassMap<WaterSampleEntity>
{
    /// <summary>
    /// Настройка маппинга: таблица water_samples, ионный состав пробы.
    /// </summary>
    public WaterSampleMap()
    {
        Table("water_samples");
        Id(x => x.Id).Column("id").GeneratedBy.Sequence("water_samples_seq");                 // PK, sequence
        References(x => x.ClusterStation).Column("station_id");                                // FK → cluster_stations, насосная станция пробы
        References(x => x.OilField).Column("oil_field_id");                                    // FK → oil_fields, месторождение пробы
        References(x => x.DevTarget).Column("dev_target_id");                                  // FK → dev_targets, объект разработки
        References(x => x.Well).Column("well_id");                                             // FK → wells, скважина отбора
        Map(x => x.WaterType).Column("water_type")                                             // Тип воды (Reservoir / Injection)
            .CustomSqlType("smallint")
            .CustomType<WaterType>()
            .Not.Nullable();
        Map(x => x.SampledAt).Column("sampled_at").Not.Nullable();                             // Дата и время отбора пробы
        Map(x => x.Chloride).Column("chloride");                                               // Cl⁻ (мг/л)
        Map(x => x.Carbonate).Column("carbonate");                                             // CO₃²⁻ (мг/л)
        Map(x => x.Bicarbonate).Column("bicarbonate");                                         // HCO₃⁻ (мг/л)
        Map(x => x.Calcium).Column("calcium");                                                 // Ca²⁺ (мг/л)
        Map(x => x.Magnesium).Column("magnesium");                                             // Mg²⁺ (мг/л)
        Map(x => x.Sodium).Column("sodium");                                                   // Na⁺ (мг/л)
        Map(x => x.Sulfate).Column("sulfate");                                                 // SO₄²⁻ (мг/л)
        Map(x => x.CreatedAt).Column("created_at");                                            // Дата создания записи
        Map(x => x.UpdatedAt).Column("updated_at");                                            // Дата последнего обновления записи
        Map(x => x.CreatedBy).Column("created_by");                                            // Автор создания
        Map(x => x.UpdatedBy).Column("updated_by");                                            // Автор последнего изменения
    }
}
