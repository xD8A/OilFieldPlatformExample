using OilFieldPlatform.Domain.Entities.Common;
using FluentNHibernate.Mapping;

namespace OilFieldPlatform.Infrastructure.Mapping.Common;

/// <summary>
/// NHibernate-маппинг сущности OilFieldEntity (месторождение).
/// </summary>
public sealed class OilFieldMap : ClassMap<OilFieldEntity>
{
    /// <summary>
    /// Настройка маппинга: таблица oil_fields, коллекции Wells и DevTargets.
    /// </summary>
    public OilFieldMap()
    {
        Table("oil_fields");
        Id(x => x.Id).Column("id").GeneratedBy.Sequence("oil_fields_seq");                   // PK, sequence
        Map(x => x.Name).Column("name").Not.Nullable();                                        // Наименование месторождения
        HasMany(x => x.Wells)                                                                  // Скважины месторождения
            .KeyColumn("oil_field_id")                                                         // FK → oil_fields в таблице wells
            .Inverse()                                                                         // Сторона-владелец — WellEntity
            .Cascade.AllDeleteOrphan()                                                         // Каскад: все операции + удаление сирот
            .AsSet();                                                                          // Тип коллекции — ISet
        HasMany(x => x.DevTargets)                                                             // Объекты разработки месторождения
            .KeyColumn("oil_field_id")                                                         // FK → oil_fields в таблице dev_targets
            .Inverse()                                                                         // Сторона-владелец — DevTargetEntity
            .Cascade.AllDeleteOrphan()                                                         // Каскад: все операции + удаление сирот
            .AsSet();                                                                          // Тип коллекции — ISet
        Map(x => x.CreatedAt).Column("created_at");                                            // Дата создания записи
        Map(x => x.UpdatedAt).Column("updated_at");                                            // Дата последнего обновления записи
        Map(x => x.CreatedBy).Column("created_by");                                            // Автор создания
        Map(x => x.UpdatedBy).Column("updated_by");                                            // Автор последнего изменения
    }
}
