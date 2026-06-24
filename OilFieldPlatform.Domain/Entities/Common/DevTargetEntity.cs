using OilFieldPlatform.Domain.Entities.ABC;

namespace OilFieldPlatform.Domain.Entities.Common;

/// <summary>
/// Объект разработки.
/// </summary>
public class DevTargetEntity : ABCNamedEntity<DevTargetEntity>
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="oilField">Месторождение, к которому относится объект разработки.</param>
    /// <param name="name">Наименование объекта разработки.</param>
    /// <param name="id">Идентификатор (опционально).</param>
    public DevTargetEntity(OilFieldEntity oilField, string name, long? id = null) : base(name, id)
    {
        OilField = oilField;
    }

    /// <summary>
    /// Конструктор для NHibernate.
    /// </summary>
    protected DevTargetEntity() : base(string.Empty)
    {
        OilField = null!;
    }

    /// <summary>
    /// Месторождение, к которому относится объект разработки.
    /// </summary>
    public virtual OilFieldEntity OilField { get; set; }
}
