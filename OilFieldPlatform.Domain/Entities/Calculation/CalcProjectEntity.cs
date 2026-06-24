using OilFieldPlatform.Domain.Entities.ABC;
using OilFieldPlatform.Domain.Entities.Common;

namespace OilFieldPlatform.Domain.Entities.Calculation;

/// <summary>
/// Расчётный проект.
/// </summary>
public class CalcProjectEntity : ABCNamedEntity<CalcProjectEntity>
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="oilField">Месторождение, по которому производится расчёт.</param>
    /// <param name="devTarget">Объект разработки, по которому производится расчёт.</param>
    /// <param name="name">Наименование проекта.</param>
    /// <param name="id">Идентификатор (опционально).</param>
    public CalcProjectEntity(OilFieldEntity oilField, DevTargetEntity devTarget, string name, long? id = null) : base(name, id)
    {
        OilField = oilField;
        DevTarget = devTarget;
    }

    /// <summary>
    /// Конструктор для NHibernate.
    /// </summary>
    protected CalcProjectEntity() : base(string.Empty)
    {
        OilField = null!;
        DevTarget = null!;
    }

    /// <summary>
    /// Месторождение, по которому производится расчёт в проекте.
    /// </summary>
    public virtual OilFieldEntity OilField { get; set; }

    /// <summary>
    /// Объект разработки, по которому производится расчёт в проекте.
    /// </summary>
    public virtual DevTargetEntity DevTarget { get; set; }

    /// <summary>
    /// Пробы воды в проекте.
    /// </summary>
    public virtual ISet<CalcWaterSampleEntity> WaterSamples { get; set; } = new HashSet<CalcWaterSampleEntity>();
}
