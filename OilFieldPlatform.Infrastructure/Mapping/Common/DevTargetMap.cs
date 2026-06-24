using OilFieldPlatform.Domain.Entities.Common;
using FluentNHibernate.Mapping;

namespace OilFieldPlatform.Infrastructure.Mapping.Common;

/// <summary>
/// NHibernate-маппинг сущности DevTargetEntity (объект разработки).
/// </summary>
public sealed class DevTargetMap : ClassMap<DevTargetEntity>
{
    /// <summary>
    /// Настройка маппинга: таблица dev_targets.
    /// </summary>
    public DevTargetMap()
    {
        Table("dev_targets");
        Id(x => x.Id).Column("id").GeneratedBy.Sequence("dev_targets_seq");               // PK, sequence
        References(x => x.OilField).Column("oil_field_id").Not.Nullable();                  // FK → oil_fields, месторождение объекта разработки
        Map(x => x.Name).Column("name").Not.Nullable();                                      // Наименование объекта разработки
        Map(x => x.CreatedAt).Column("created_at");                                          // Дата создания записи
        Map(x => x.UpdatedAt).Column("updated_at");                                          // Дата последнего обновления записи
        Map(x => x.CreatedBy).Column("created_by");                                          // Автор создания
        Map(x => x.UpdatedBy).Column("updated_by");                                          // Автор последнего изменения
    }
}
