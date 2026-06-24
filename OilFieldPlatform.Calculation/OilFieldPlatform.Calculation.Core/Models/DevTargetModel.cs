namespace OilFieldPlatform.Calculation.Core.Models;

/// <summary>Объект разработки (ссылка).</summary>
public class DevTargetModel
{
    /// <summary>Идентификатор объекта разработки.</summary>
    public long Id { get; }

    /// <summary>Наименование объекта разработки.</summary>
    public string Name { get; }

    /// <summary>Конструктор.</summary>
    /// <param name="name">Наименование объекта разработки.</param>
    /// <param name="id">Идентификатор (опционально).</param>
    public DevTargetModel(string name, long id)
    {
        Name = name;
        Id = id;
    }
}
