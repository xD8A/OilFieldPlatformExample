using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json.Serialization;
using OilFieldPlatform.Calculation.Core.Models;

namespace OilFieldPlatform.Calculation.Core.States;

/// <summary>Состояние приложения: текущий проект.</summary>
public sealed class ApplicationState : IDisposable
{
    private readonly BehaviorSubject<ProjectModel?> _projectSubject = new(null);
    private ProjectModel? _project;
    private bool _disposed;

    /// <summary>Наблюдаемый проект.</summary>
    [JsonIgnore]
    public IObservable<ProjectModel?> ProjectAsObservable => _projectSubject.AsObservable();

    /// <summary>Текущий расчётный проект.</summary>
    public ProjectModel? Project
    {
        get => _project;
        set
        {
            if (_project == value)
                return;

            var oldProject = _project;
            _project = value;
            _projectSubject.OnNext(value);
            oldProject?.Dispose();
        }
    }

    /// <summary>Флаг наличия несохранённых изменений в текущем проекте.</summary>
    public bool IsChanged => _project?.IsChanged ?? false;

    /// <summary>Освобождение ресурсов.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _project?.Dispose();
            _projectSubject.Dispose();
        }
        _disposed = true;
    }
}
