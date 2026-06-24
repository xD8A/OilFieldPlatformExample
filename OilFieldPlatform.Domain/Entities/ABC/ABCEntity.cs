#pragma warning disable S101, S4035 // NHibernate base class: ABC naming, IEquatable on unsealed
using OilFieldPlatform.Domain.Interfaces;

namespace OilFieldPlatform.Domain.Entities.ABC;

/// <summary>
/// Базовая абстрактная сущность с идентификатором, аудитом и равенством по Id.
/// </summary>
public abstract class ABCEntity<TSelf> : IEntity<TSelf> where TSelf : ABCEntity<TSelf>
{
    /// <summary>
    /// Конструктор для создания сущности в прикладном коде.
    /// </summary>
    /// <param name="id">Идентификатор сущности (опционально, для <c>null</c> генерируется БД).</param>
    protected ABCEntity(long? id = null)
    {
        Id = id;
    }

    /// <summary>
    /// Идентификатор сущности.
    /// </summary>
    public virtual long? Id { get; set; }

    /// <summary>
    /// Дата создания записи.
    /// </summary>
    public virtual DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления записи.
    /// </summary>
    public virtual DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Кем создана запись.
    /// </summary>
    public virtual string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Кем последний раз обновлена запись.
    /// </summary>
    public virtual string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Сравнение сущностей по Id.
    /// </summary>
    /// <param name="other">Другая сущность для сравнения.</param>
    /// <returns><c>true</c>, если Id совпадают и не null.</returns>
    public virtual bool Equals(TSelf? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Id is null || other.Id is null) return false;
        return Id == other.Id;
    }

    /// <summary>
    /// Сравнение с произвольным объектом.
    /// </summary>
    /// <param name="obj">Объект для сравнения.</param>
    /// <returns><c>true</c>, если obj является сущностью того же типа с совпадающим Id.</returns>
    public override bool Equals(object? obj) => obj is TSelf other && Equals(other);

    /// <summary>
    /// Хэш-код сущности на основе Id.
    /// </summary>
    /// <returns>Хэш-код Id, или 0 если Id не задан.</returns>
    public override int GetHashCode() => Id?.GetHashCode() ?? 0;
}
