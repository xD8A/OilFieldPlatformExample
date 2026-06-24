using System.Reactive.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using OilFieldPlatform.Calculation.Core.Services;
using OilFieldPlatform.Calculation.Core.States;
using OilFieldPlatform.Calculation.Core.States.UI;
using OilFieldPlatform.Calculation.Server.Schemas;
using OilFieldPlatform.Calculation.Server.Schemas.Requests;
using OilFieldPlatform.Calculation.Server.Schemas.Responses;
using OilFieldPlatform.Domain.Projections.Calculation;

namespace OilFieldPlatform.Calculation.Server.Controllers;

/// <summary>Контроллер состояния приложения и управления проектами.</summary>
public class ApplicationController : IWebSocketController, IDisposable
{
    private readonly ManageProjectService _manageProjectService;
    private readonly ListProjectService _listProjectService;
    private readonly ApplicationState _state;
    private readonly ApplicationHeaderState _headerState;
    private readonly ILogger _logger;
    private readonly IDisposable _isChangedSubscription;
    private bool _disposed;

    /// <summary>Конструктор.</summary>
    /// <param name="manageProjectService">Сервис управления проектами.</param>
    /// <param name="listProjectService">Сервис получения списка проектов.</param>
    /// <param name="state">Состояние приложения.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public ApplicationController(
        ManageProjectService manageProjectService,
        ListProjectService listProjectService,
        ApplicationState state,
        ILogger<ApplicationController>? logger = null)
    {
        _manageProjectService = manageProjectService;
        _listProjectService = listProjectService;
        _state = state;
        _headerState = new ApplicationHeaderState(state);
        _logger = (ILogger?)logger ?? NullLogger.Instance;

        _isChangedSubscription = IsChangedAsObservable
            .Subscribe(isChanged =>
            {
                var response = new ApplicationIsChangedResponse { IsChanged = isChanged };
                OnChanged?.Invoke(this, response);
            });
    }

    /// <summary>Флаг наличия несохранённых изменений (реактивный).</summary>
    public IObservable<bool> IsChangedAsObservable =>
        _state.ProjectAsObservable
            .Select(p => p?.IsChangedAsObservable ?? Observable.Return(false))
            .Switch();

    /// <summary>Получить текущее состояние приложения.</summary>
    /// <returns>Состояние приложения.</returns>
    public ApplicationHeaderState GetState()
    {
        _logger.LogDebug("Getting application state");
        return _headerState;
    }

    /// <summary>Получить список проекций всех проектов.</summary>
    /// <returns>Список проекций проектов.</returns>
    private async Task<IList<CalcProjectProjection>> ListProjects()
    {
        _logger.LogDebug("Listing projects");
        return await _listProjectService.GetAllAsync();
    }

    /// <summary>Открыть проект по идентификатору.</summary>
    /// <param name="id">Идентификатор проекта.</param>
    private void OpenProject(long id)
    {
        _logger.LogDebug("Opening project Id: {ProjectId}", id);
        var project = _manageProjectService.Load(id);
        if (project is null)
        {
            _logger.LogWarning("Project not found for opening: {ProjectId}", id);
            return;
        }
        _state.Project = project;
        _logger.LogInformation("Project opened: {ProjectId}", id);
    }

    /// <summary>Сохранить текущий проект.</summary>
    private void SaveProject()
    {
        var project = _state.Project;
        if (project is null)
        {
            _logger.LogWarning("No project to save");
            return;
        }
        _logger.LogDebug("Saving current project Id: {ProjectId}", project.Id);
        _manageProjectService.Save(project);
        project.MarkUnchanged();
        _logger.LogInformation("Current project saved: {ProjectId}", project.Id);
    }

    /// <summary>Закрыть текущий проект.</summary>
    public void CloseProject()
    {
        _logger.LogDebug("Closing project");
        var project = _state.Project;
        if (project is not null)
            project.MarkUnchanged();
        _state.Project = null;
        _logger.LogInformation("Project closed");
    }

    /// <summary>Обработать запрос списка проектов.</summary>
    public ApplicationListProjectsResponse HandleListProject()
    {
        var projects = ListProjects().GetAwaiter().GetResult();
        return new ApplicationListProjectsResponse { Projects = projects };
    }

    /// <summary>Обработать запрос состояния приложения.</summary>
    public ApplicationStateResponse HandleGetAppState()
    {
        return new ApplicationStateResponse { State = _headerState };
    }

    /// <summary>Обработать запрос открытия проекта.</summary>
    public ApplicationProjectOpenedResponse HandleOpenProject(ApplicationOpenProjectRequest request)
    {
        OpenProject(request.Id);
        return new ApplicationProjectOpenedResponse();
    }

    /// <summary>Обработать запрос сохранения проекта.</summary>
    public ApplicationProjectSavedResponse HandleSaveProject()
    {
        SaveProject();
        return new ApplicationProjectSavedResponse();
    }

    /// <summary>Обработать запрос закрытия проекта.</summary>
    public ApplicationProjectClosedResponse HandleCloseProject()
    {
        CloseProject();
        return new ApplicationProjectClosedResponse();
    }

    /// <summary>Обработать запрос.</summary>
    public IWebSocketResponse? HandleRequest(IWebSocketRequest request)
    {
        IWebSocketResponse? response = request switch
        {
            ApplicationListProjectRequest _ => (IWebSocketResponse)HandleListProject(),
            ApplicationGetStateRequest _ => (IWebSocketResponse)HandleGetAppState(),
            ApplicationOpenProjectRequest r => (IWebSocketResponse)HandleOpenProject(r),
            ApplicationSaveProjectRequest _ => (IWebSocketResponse)HandleSaveProject(),
            ApplicationCloseProjectRequest _ => (IWebSocketResponse)HandleCloseProject(),
            _ => null
        };
        return response;
    }

    /// <summary>Опубликовать лог-сообщение, если подключение активно.</summary>
    /// <param name="response">Лог-сообщение.</param>
    public void PublishLog(LogResponse response)
    {
        OnChanged?.Invoke(this, response);
    }

    /// <summary>Событие изменения.</summary>
    public event EventHandler<IWebSocketResponse>? OnChanged;

    /// <summary>Освобождение ресурсов.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Освобождение ресурсов.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            _isChangedSubscription.Dispose();
        _disposed = true;
    }
}
