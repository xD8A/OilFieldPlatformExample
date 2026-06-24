using OilFieldPlatform.Domain.Entities.Common;
using FluentNHibernate.Mapping;

namespace OilFieldPlatform.Infrastructure.Mapping.Common;

/// <summary>
/// NHibernate-маппинг сущности WellEntity (скважина).
/// </summary>
public sealed class WellMap : ClassMap<WellEntity>
{
    /// <summary>
    /// Настройка маппинга: таблица wells.
    /// </summary>
    public WellMap()
    {
        Table("wells");
        Id(x => x.Id).Column("id").GeneratedBy.Sequence("wells_seq");                         // PK, sequence
        References(x => x.OilField).Column("oil_field_id").Not.Nullable();                     // FK → oil_fields, месторождение скважины
        Map(x => x.Name).Column("name").Not.Nullable();                                        // Номер / наименование скважины
        Map(x => x.CreatedAt).Column("created_at");                                            // Дата создания записи
        Map(x => x.UpdatedAt).Column("updated_at");                                            // Дата последнего обновления записи
        Map(x => x.CreatedBy).Column("created_by");                                            // Автор создания
        Map(x => x.UpdatedBy).Column("updated_by");                                            // Автор последнего изменения
    }
}
