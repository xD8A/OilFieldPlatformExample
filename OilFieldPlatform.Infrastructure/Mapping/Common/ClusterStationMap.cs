using OilFieldPlatform.Domain.Entities.Common;
using OilFieldPlatform.Domain.Enums;
using FluentNHibernate.Mapping;

namespace OilFieldPlatform.Infrastructure.Mapping.Common;

/// <summary>
/// NHibernate-маппинг сущности ClusterStationEntity (кустовая / дожимная насосная станция).
/// </summary>
public sealed class ClusterStationMap : ClassMap<ClusterStationEntity>
{
    /// <summary>
    /// Настройка маппинга: таблица cluster_stations.
    /// </summary>
    public ClusterStationMap()
    {
        Table("cluster_stations");
        Id(x => x.Id).Column("id").GeneratedBy.Sequence("cluster_stations_seq");               // PK, sequence
        Map(x => x.Name).Column("name").Not.Nullable();                                          // Наименование станции (КНС / ДНС)
        Map(x => x.StationType).Column("station_type")                                           // Тип станции: КНС или ДНС
            .CustomSqlType("smallint")
            .CustomType<ClusterStationType>()
            .Not.Nullable();
        Map(x => x.CreatedAt).Column("created_at");                                              // Дата создания записи
        Map(x => x.UpdatedAt).Column("updated_at");                                              // Дата последнего обновления записи
        Map(x => x.CreatedBy).Column("created_by");                                              // Автор создания
        Map(x => x.UpdatedBy).Column("updated_by");                                              // Автор последнего изменения
    }
}
