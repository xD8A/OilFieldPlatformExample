using OilFieldPlatform.Domain.Entities.ABC;
using OilFieldPlatform.Domain.Enums;

namespace OilFieldPlatform.Domain.Entities.Common;

/// <summary>
/// Кустовая / дожимная насосная станция (КНС / ДНС).
/// Предназначена для сбора, сепарации и транспортировки продукции скважин,
/// а также для поддержания пластового давления путём закачки воды.
/// </summary>
public class ClusterStationEntity : ABCNamedEntity<ClusterStationEntity>
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="stationType">Тип станции: КНС или ДНС.</param>
    /// <param name="name">Наименование станции.</param>
    /// <param name="id">Идентификатор (опционально).</param>
    public ClusterStationEntity(ClusterStationType stationType, string name, long? id = null) : base(name, id)
    {
        StationType = stationType;
    }

    /// <summary>
    /// Конструктор для NHibernate.
    /// </summary>
    protected ClusterStationEntity() : base(string.Empty)
    {
    }

    /// <summary>
    /// Тип насосной станции: КНС / ДНС.
    /// </summary>
    public virtual ClusterStationType StationType { get; set; }
}
