using OilFieldPlatform.Domain.Entities.ABC;

namespace OilFieldPlatform.Domain.Entities.Common;

/// <summary>
/// Месторождение.
/// </summary>
public class OilFieldEntity : ABCNamedEntity<OilFieldEntity>
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="name">Наименование месторождения.</param>
    /// <param name="id">Идентификатор (опционально).</param>
    public OilFieldEntity(string name, long? id = null) : base(name, id)
    {
        Wells = new HashSet<WellEntity>();
        DevTargets = new HashSet<DevTargetEntity>();
    }

    /// <summary>
    /// Конструктор для NHibernate.
    /// </summary>
    protected OilFieldEntity() : base(string.Empty)
    {
        Wells = null!;
        DevTargets = null!;
    }

    /// <summary>
    /// Скважины месторождения.
    /// </summary>
    public virtual ISet<WellEntity> Wells { get; set; }

    /// <summary>
    /// Объекты разработки месторождения.
    /// </summary>
    public virtual ISet<DevTargetEntity> DevTargets { get; set; }
}
