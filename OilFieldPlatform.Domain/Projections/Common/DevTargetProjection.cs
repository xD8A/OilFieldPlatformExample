namespace OilFieldPlatform.Domain.Projections.Common;

/// <summary>Проекция объекта разработки (Id + Name).</summary>
public sealed class DevTargetProjection
{
    /// <summary>Идентификатор объекта разработки.</summary>
    public long Id { get; set; }

    /// <summary>Наименование объекта разработки.</summary>
    public string Name { get; set; } = string.Empty;
}
