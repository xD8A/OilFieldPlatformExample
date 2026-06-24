namespace OilFieldPlatform.Domain.Interfaces;

/// <summary>
/// Базовый интерфейс сущности с идентификатором и аудитом.
/// </summary>
public interface IEntity<TSelf> : IAuditable, IEquatable<TSelf> where TSelf : IEntity<TSelf>
{
    /// <summary>Идентификатор сущности.</summary>
    long? Id { get; set; }
}
