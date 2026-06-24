namespace OilFieldPlatform.Domain.Projections.Common;

/// <summary>Проекция скважины (Id + Name).</summary>
public sealed class WellProjection
{
    /// <summary>Идентификатор скважины.</summary>
    public long Id { get; set; }

    /// <summary>Номер / наименование скважины.</summary>
    public string Name { get; set; } = string.Empty;
}
