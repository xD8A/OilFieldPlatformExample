namespace OilFieldPlatform.Domain.Projections.Common;

/// <summary>Проекция насосной станции (Id + Name).</summary>
public sealed class ClusterStationProjection
{
    /// <summary>Идентификатор станции.</summary>
    public long Id { get; set; }

    /// <summary>Наименование станции (КНС / ДНС).</summary>
    public string Name { get; set; } = string.Empty;
}
