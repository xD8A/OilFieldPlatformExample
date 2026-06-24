using OilFieldPlatform.Domain.Entities.Calculation;
using FluentNHibernate.Mapping;

namespace OilFieldPlatform.Infrastructure.Mapping.Calculation;

/// <summary>
/// NHibernate-маппинг сущности CalcProjectEntity (расчётный проект).
/// </summary>
public sealed class CalcProjectMap : ClassMap<CalcProjectEntity>
{
    /// <summary>
    /// Настройка маппинга: таблица calc_projects, коллекция WaterSamples.
    /// </summary>
    public CalcProjectMap()
    {
        Table("calc_projects");
        Id(x => x.Id).Column("id").GeneratedBy.Sequence("calc_projects_seq");                   // PK, sequence
        References(x => x.OilField).Column("oil_field_id").Not.Nullable();                       // FK → oil_fields, месторождение проекта
        References(x => x.DevTarget).Column("dev_target_id").Not.Nullable();                     // FK → dev_targets, объект разработки проекта
        HasMany(x => x.WaterSamples)                                                             // Пробы воды в проекте
            .KeyColumn("calc_project_id")                                                        // FK → calc_projects в таблице calc_water_samples
            .Inverse()                                                                           // Сторона-владелец — CalcWaterSampleEntity
            .Cascade.AllDeleteOrphan()                                                           // Каскад: все операции + удаление сирот
            .AsSet();                                                                            // Тип коллекции — ISet
        Map(x => x.Name).Column("name").Not.Nullable();                                          // Наименование проекта
        Map(x => x.CreatedAt).Column("created_at");                                              // Дата создания записи
        Map(x => x.UpdatedAt).Column("updated_at");                                              // Дата последнего обновления записи
        Map(x => x.CreatedBy).Column("created_by");                                              // Автор создания
        Map(x => x.UpdatedBy).Column("updated_by");                                              // Автор последнего изменения
    }
}
