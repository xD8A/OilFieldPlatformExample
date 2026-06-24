using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NHibernate;
using OilFieldPlatform.Calculation.Core.Models;
using OilFieldPlatform.Domain.Entities.Calculation;
using OilFieldPlatform.Infrastructure.Repositories.Calculation;

namespace OilFieldPlatform.Calculation.Core.Services;

/// <summary>Сервис управления расчётным проектом.</summary>
public class ManageProjectService
{
    private readonly CalcProjectRepository _repository;
    private readonly ISession _session;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    /// <summary>Конструктор.</summary>
    /// <param name="repository">Репозиторий расчётных проектов.</param>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="mapper">AutoMapper.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public ManageProjectService(
        CalcProjectRepository repository,
        ISession session,
        IMapper mapper,
        ILogger<ManageProjectService>? logger = null)
    {
        _repository = repository;
        _session = session;
        _mapper = mapper;
        _logger = (ILogger?)logger ?? NullLogger.Instance;
    }

    /// <summary>Загрузить проект по Id.</summary>
    /// <param name="id">Идентификатор проекта.</param>
    /// <returns>Проект или null.</returns>
    public ProjectModel? Load(long id)
    {
        _logger.LogDebug("Loading project by Id: {ProjectId}", id);
        var entity = _repository.GetById(id);
        if (entity is null)
        {
            _logger.LogWarning("Project not found by Id: {ProjectId}", id);
            return null;
        }

        NHibernateUtil.Initialize(entity.WaterSamples);
        foreach (var ws in entity.WaterSamples)
            NHibernateUtil.Initialize(ws.Equivalent);

        var project = _mapper.Map<ProjectModel>(entity, opt => { });
        _logger.LogInformation("Loaded project Id: {ProjectId}", id);
        return project;
    }

    /// <summary>Сохранить проект (добавить или обновить).</summary>
    /// <param name="project">Проект для сохранения.</param>
    public void Save(ProjectModel project)
    {
        _logger.LogDebug("Saving project Id: {ProjectId}", project.Id);

        var waterSampleMap = new Dictionary<CalcWaterSampleEntity, WaterSampleModel>();
        using var tx = _session.BeginTransaction();
        try
        {
            var entity = _mapper.Map<CalcProjectEntity>(project, opt =>
            {
                opt.Items["Session"] = _session;
                opt.Items["WaterSampleMap"] = waterSampleMap;
            });

            if (entity.Id is null)
            {
                _logger.LogDebug("Adding new project");
                _repository.Add(entity);
            }
            else
            {
                _logger.LogDebug("Updating existing project Id: {ProjectId}", entity.Id);
                _repository.Update(entity);
            }

            tx.Commit();
            ApplyIds(entity, project, waterSampleMap);
            _logger.LogInformation("Project saved: Id {ProjectId}", project.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Transaction rolled back for project Id: {ProjectId}", project.Id);
            tx.Rollback();
            throw new InvalidOperationException($"Failed to save project Id: {project.Id}", ex);
        }
    }


    private static void ApplyIds(
        CalcProjectEntity entity,
        ProjectModel model,
        Dictionary<CalcWaterSampleEntity, WaterSampleModel> waterSampleMap)
    {
        model.Id = entity.Id;

        foreach (var ws in entity.WaterSamples)
            if (waterSampleMap.TryGetValue(ws, out var modelSample))
                modelSample.Id = ws.Id;
    }

    /// <summary>Удалить проект по Id.</summary>
    /// <param name="id">Идентификатор проекта.</param>
    public void Delete(long id)
    {
        _logger.LogDebug("Deleting project by Id: {ProjectId}", id);
        using var tx = _session.BeginTransaction();
        try
        {
            _repository.Delete(id);
            tx.Commit();
            _logger.LogInformation("Project deleted: Id {ProjectId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Transaction rolled back on delete for project Id: {ProjectId}", id);
            tx.Rollback();
            throw new InvalidOperationException($"Failed to delete project Id: {id}", ex);
        }
    }
}
