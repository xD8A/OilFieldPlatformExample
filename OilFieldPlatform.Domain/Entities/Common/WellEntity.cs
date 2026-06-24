using OilFieldPlatform.Domain.Entities.ABC;

namespace OilFieldPlatform.Domain.Entities.Common;

/// <summary>
/// Скважина.
/// </summary>
public class WellEntity : ABCNamedEntity<WellEntity>
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="oilField">Месторождение, к которому относится скважина.</param>
    /// <param name="name">Номер / наименование скважины.</param>
    /// <param name="id">Идентификатор (опционально).</param>
    public WellEntity(OilFieldEntity oilField, string name, long? id = null) : base(name, id)
    {
        OilField = oilField;
    }

    /// <summary>
    /// Конструктор для NHibernate.
    /// </summary>
    protected WellEntity() : base(string.Empty)
    {
        OilField = null!;
    }

    /// <summary>
    /// Месторождение, к которому относится скважина.
    /// </summary>
    public virtual OilFieldEntity OilField { get; set; }
}
