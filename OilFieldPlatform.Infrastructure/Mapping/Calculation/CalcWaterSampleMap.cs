#pragma warning disable CS8602 // x in PropertyRef expression is never null — used only via reflection
using OilFieldPlatform.Domain.Entities.Calculation;
using OilFieldPlatform.Domain.Enums;
using FluentNHibernate.Mapping;

namespace OilFieldPlatform.Infrastructure.Mapping.Calculation;

/// <summary>
/// NHibernate-маппинг сущности CalcWaterSampleEntity (проба воды в расчётном проекте).
/// </summary>
public sealed class CalcWaterSampleMap : ClassMap<CalcWaterSampleEntity>
{
    /// <summary>
    /// Настройка маппинга: таблица calc_water_samples, ионный состав, эквиваленты, ссылка на исходную пробу.
    /// </summary>
    public CalcWaterSampleMap()
    {
        Table("calc_water_samples");
        Id(x => x.Id).Column("id").GeneratedBy.Sequence("calc_water_samples_seq");             // PK, sequence
        References(x => x.Project).Column("calc_project_id").Not.Nullable();                    // FK → calc_projects, расчётный проект
        References(x => x.SourceSample).Column("source_sample_id");                             // FK → water_samples, исходная проба
        Map(x => x.WellName).Column("well_name");                                               // Номер / наименование скважины (текст)
        Map(x => x.ClusterStationName).Column("station_name");                                  // Наименование насосной станции (КНС / ДНС)
        Map(x => x.SampledAt).Column("sampled_at").Not.Nullable();                              // Дата и время отбора пробы
        Map(x => x.WaterType).Column("water_type").CustomType<EnumIntType<WaterType>>().Not.Nullable();    // Тип воды (Reservoir / Injection)
        Map(x => x.Chloride).Column("chloride");                                                // Cl⁻ (мг/л)
        Map(x => x.Carbonate).Column("carbonate");                                              // CO₃²⁻ (мг/л)
        Map(x => x.Bicarbonate).Column("bicarbonate");                                          // HCO₃⁻ (мг/л)
        Map(x => x.Sulfate).Column("sulfate");                                                  // SO₄²⁻ (мг/л)
        Map(x => x.Calcium).Column("calcium");                                                  // Ca²⁺ (мг/л)
        Map(x => x.Magnesium).Column("magnesium");                                              // Mg²⁺ (мг/л)
        Map(x => x.Sodium).Column("sodium");                                                    // Na⁺ (мг/л)
        HasOne(x => x.Equivalent)                                                               // Расчётные эквиваленты ионов
            .PropertyRef(x => x.WaterSample)                                                    // Ссылка в дочерней таблице — WaterSample
            .Cascade.All();                                                                     // Каскад: все операции
        Map(x => x.CreatedAt).Column("created_at");                                             // Дата создания записи
        Map(x => x.UpdatedAt).Column("updated_at");                                             // Дата последнего обновления записи
        Map(x => x.CreatedBy).Column("created_by");                                             // Автор создания
        Map(x => x.UpdatedBy).Column("updated_by");                                             // Автор последнего изменения
    }
}
