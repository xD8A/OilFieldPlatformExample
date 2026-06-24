namespace OilFieldPlatform.Domain.Projections.Common;

/// <summary>Проекция месторождения (Id + Name).</summary>
public sealed class OilFieldProjection
{
    /// <summary>Идентификатор месторождения.</summary>
    public long Id { get; set; }

    /// <summary>Наименование месторождения.</summary>
    public string Name { get; set; } = string.Empty;
}
