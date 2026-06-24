namespace OilFieldPlatform.Domain.Projections.Calculation;

/// <summary>Проекция расчётного проекта (Id + Name).</summary>
public sealed class CalcProjectProjection
{
    /// <summary>Идентификатор проекта.</summary>
    public long Id { get; set; }

    /// <summary>Наименование проекта.</summary>
    public string Name { get; set; } = string.Empty;
}
