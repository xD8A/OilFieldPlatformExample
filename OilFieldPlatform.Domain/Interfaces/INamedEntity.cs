namespace OilFieldPlatform.Domain.Interfaces;

/// <summary>
/// Интерфейс поименованной сущности (наследует IEntity и добавляет Name).
/// </summary>
public interface INamedEntity<TSelf> : IEntity<TSelf> where TSelf : INamedEntity<TSelf>
{
    /// <summary>Наименование сущности.</summary>
    string Name { get; set; }
}
