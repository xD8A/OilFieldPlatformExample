#pragma warning disable S101 // NHibernate base class: ABC naming
using OilFieldPlatform.Domain.Interfaces;

namespace OilFieldPlatform.Domain.Entities.ABC;

/// <summary>
/// Абстрактная сущность с именем.
/// </summary>
public abstract class ABCNamedEntity<TSelf> : ABCEntity<TSelf>, INamedEntity<TSelf> where TSelf : ABCNamedEntity<TSelf>
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="name">Наименование сущности (опционально).</param>
    /// <param name="id">Идентификатор (опционально).</param>
    protected ABCNamedEntity(string name = "", long? id = null) : base(id)
    {
        Name = name;
    }

    /// <summary>
    /// Название сущности.
    /// </summary>
    public virtual string Name { get; set; }
}
