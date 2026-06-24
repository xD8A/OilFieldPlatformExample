using System.Text.Json.Serialization;
using OilFieldPlatform.Calculation.Core.Models;

namespace OilFieldPlatform.Calculation.Core.Proxies;

/// <summary>Прокси для расчётного проекта.</summary>
public sealed class ProjectProxyModel
{
    /// <summary>Конструктор прокси для проекта.</summary>
    /// <param name="project">Расчётный проект.</param>
    public ProjectProxyModel(ProjectModel project)
    {
        Project = project;
    }

    /// <summary>Расчётный проект.</summary>
    [JsonIgnore]
    public ProjectModel Project { get; }

    /// <summary>Название проекта.</summary>
    public string Name => Project.Name;

    /// <summary>Флаг наличия несохранённых изменений.</summary>
    public bool IsChanged => Project.IsChanged;
}
