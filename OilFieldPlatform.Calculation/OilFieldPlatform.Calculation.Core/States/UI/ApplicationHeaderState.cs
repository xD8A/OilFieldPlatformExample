using OilFieldPlatform.Calculation.Core.Proxies;

namespace OilFieldPlatform.Calculation.Core.States.UI;

/// <summary>Состояние приложения (UI): текущий проект.</summary>
public sealed class ApplicationHeaderState : IDisposable
{
    private readonly IDisposable _subscription;
    private bool _disposed;

    /// <summary>Конструктор.</summary>
    /// <param name="appState">Состояние приложения.</param>
    public ApplicationHeaderState(ApplicationState appState)
    {
        if (appState.Project is not null)
            Project = new ProjectProxyModel(appState.Project);

        _subscription = appState.ProjectAsObservable
            .Subscribe(project =>
            {
                Project = project is not null
                    ? new ProjectProxyModel(project)
                    : null;
            });
    }

    /// <summary>Текущий расчётный проект.</summary>
    public ProjectProxyModel? Project { get; private set; }

    /// <summary>Освобождение ресурсов.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _subscription.Dispose();
        Project = null;
        _disposed = true;
    }
}
