namespace OilFieldPlatform.Calculation.Core.Models;

/// <summary>Месторождение (ссылка).</summary>
public class OilFieldModel
{
    /// <summary>Идентификатор месторождения.</summary>
    public long Id { get; }

    /// <summary>Наименование месторождения.</summary>
    public string Name { get; }

    /// <summary>Конструктор.</summary>
    /// <param name="name">Наименование месторождения.</param>
    /// <param name="id">Идентификатор месторождения.</param>
    public OilFieldModel(string name, long id)
    {
        Name = name;
        Id = id;
    }
}
