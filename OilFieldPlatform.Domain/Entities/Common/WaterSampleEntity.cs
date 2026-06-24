using OilFieldPlatform.Domain.Entities.ABC;
using OilFieldPlatform.Domain.Enums;
using OilFieldPlatform.Domain.Interfaces;

namespace OilFieldPlatform.Domain.Entities.Common;

/// <summary>
/// Проба воды.
/// </summary>
public class WaterSampleEntity : ABCEntity<WaterSampleEntity>, IAnionSample, ICationSample
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="oilField">Месторождение пробы (опционально, зависит от типа воды).</param>
    /// <param name="devTarget">Объект разработки (обязателен для Reservoir).</param>
    /// <param name="well">Скважина отбора (обязательна для Reservoir).</param>
    /// <param name="waterType">Тип воды (Reservoir / Injection).</param>
    /// <param name="sampledAt">Дата и время отбора пробы.</param>
    /// <param name="id">Идентификатор (опционально).</param>
    public WaterSampleEntity(OilFieldEntity? oilField, DevTargetEntity? devTarget, WellEntity? well, WaterType waterType, DateTime sampledAt, long? id = null) : base(id)
    {
        OilField = oilField;
        DevTarget = devTarget;
        Well = well;
        WaterType = waterType;
        SampledAt = sampledAt;
    }

    /// <summary>
    /// Конструктор для NHibernate.
    /// </summary>
    protected WaterSampleEntity() : base()
    {
    }

    /// <summary>
    /// Месторождение, к которому относится проба.
    /// Для пластовой воды (Reservoir) — месторождение определяется по скважине и объекту разработки.
    /// Для закачиваемой (Injection) — может быть задано напрямую, если насосная станция не указана.
    /// </summary>
    public virtual OilFieldEntity? OilField { get; set; }

    /// <summary>
    /// Объект разработки, к которому относится проба.
    /// Обязателен для пластовой воды (Reservoir).
    /// Для закачиваемой (Injection) может быть не указан.
    /// Должен относиться к тому же месторождению, что и скважина.
    /// </summary>
    public virtual DevTargetEntity? DevTarget { get; set; }

    /// <summary>
    /// Скважина, из которой отобрана проба.
    /// Обязательна для пластовой воды (Reservoir).
    /// Для закачиваемой (Injection) может быть не указана.
    /// Должна относиться к тому же месторождению, что и объект разработки.
    /// </summary>
    public virtual WellEntity? Well { get; set; }

    /// <summary>
    /// Насосная станция, с которой связана проба.
    /// Указывается для закачиваемой воды (Injection).
    /// Для пластовой воды (Reservoir) не задаётся.
    /// Если насосная станция не указана, должно быть задано месторождение напрямую.
    /// </summary>
    public virtual ClusterStationEntity? ClusterStation { get; set; }

    /// <summary>
    /// Тип воды: пластовая (Reservoir) или закачиваемая (Injection).
    /// </summary>
    public virtual WaterType WaterType { get; set; }

    /// <summary>
    /// Дата и время отбора пробы.
    /// </summary>
    public virtual DateTime SampledAt { get; set; }

    /// <summary>
    /// Концентрация хлорид-иона Cl⁻ (мг/л).
    /// </summary>
    public virtual double? Chloride { get; set; }

    /// <summary>
    /// Концентрация карбонат-иона CO₃²⁻ (мг/л).
    /// </summary>
    public virtual double? Carbonate { get; set; }

    /// <summary>
    /// Концентрация гидрокарбонат-иона HCO₃⁻ (мг/л).
    /// </summary>
    public virtual double? Bicarbonate { get; set; }

    /// <summary>
    /// Концентрация иона кальция Ca²⁺ (мг/л).
    /// </summary>
    public virtual double? Calcium { get; set; }

    /// <summary>
    /// Концентрация иона магния Mg²⁺ (мг/л).
    /// </summary>
    public virtual double? Magnesium { get; set; }

    /// <summary>
    /// Концентрация иона натрия Na⁺ (мг/л).
    /// </summary>
    public virtual double? Sodium { get; set; }

    /// <summary>
    /// Концентрация сульфат-иона SO₄²⁻ (мг/л).
    /// </summary>
    public virtual double? Sulfate { get; set; }
}
