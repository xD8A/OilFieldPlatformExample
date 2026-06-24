using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Calculation;
using OilFieldPlatform.Infrastructure.Repositories.ABC;
using NHibernate;
using NHibernate.Linq;

namespace OilFieldPlatform.Infrastructure.Repositories.Calculation;

/// <summary>
/// Репозиторий чтения для CalcWaterSampleEntity (проба воды в расчётном проекте).
/// </summary>
public class CalcWaterSampleReadRepository : ABCRepository<CalcWaterSampleEntity>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public CalcWaterSampleReadRepository(ISession session, ILogger<CalcWaterSampleReadRepository>? logger = null) : base(session, logger)
    {
    }

    private IQueryable<CalcWaterSampleEntity> BuildByProjectIdQuery(long projectId)
    {
        return _session.Query<CalcWaterSampleEntity>()
            .Where(s => s.Project!.Id == projectId);
    }

    /// <summary>
    /// Получить пробы по идентификатору проекта.
    /// </summary>
    /// <param name="projectId">Id расчётного проекта.</param>
    /// <returns>Список проб проекта.</returns>
    public IEnumerable<CalcWaterSampleEntity> GetByProjectId(long projectId)
    {
        _logger.LogDebug("Getting water samples by project Id: {ProjectId}", projectId);
        var result = BuildByProjectIdQuery(projectId).ToList();
        _logger.LogDebug("Found {Count} water samples for project Id: {ProjectId}", result.Count, projectId);
        return result;
    }

    /// <summary>
    /// Получить пробы по проекту (асинхронно).
    /// </summary>
    /// <param name="projectId">Id расчётного проекта.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список проб проекта.</returns>
    public async Task<IEnumerable<CalcWaterSampleEntity>> GetByProjectIdAsync(long projectId, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting water samples by project Id async: {ProjectId}", projectId);
        var result = await BuildByProjectIdQuery(projectId).ToListAsync(ct);
        _logger.LogDebug("Found {Count} water samples for project Id: {ProjectId} async", result.Count, projectId);
        return result;
    }
}
