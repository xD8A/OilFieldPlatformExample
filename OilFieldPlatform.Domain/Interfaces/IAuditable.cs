namespace OilFieldPlatform.Domain.Interfaces;

/// <summary>
/// Интерфейс аудита: даты и авторы создания/изменения записи.
/// </summary>
public interface IAuditable
{
    /// <summary>Дата создания записи.</summary>
    DateTime CreatedAt { get; set; }

    /// <summary>Дата последнего обновления записи.</summary>
    DateTime UpdatedAt { get; set; }

    /// <summary>Кем создана запись.</summary>
    string CreatedBy { get; set; }

    /// <summary>Кем последний раз обновлена запись.</summary>
    string UpdatedBy { get; set; }
}
