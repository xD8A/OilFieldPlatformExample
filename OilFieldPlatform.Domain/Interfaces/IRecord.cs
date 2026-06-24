namespace OilFieldPlatform.Domain.Interfaces;

/// <summary>
/// Интерфейс записи с аудитом и поддержкой сравнения.
/// </summary>
public interface IRecord<in TSelf> : IAuditable, IComparable<TSelf> where TSelf : IRecord<TSelf>
{
}
