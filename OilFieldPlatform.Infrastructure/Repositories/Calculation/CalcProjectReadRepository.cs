using Microsoft.Extensions.Logging;
using OilFieldPlatform.Domain.Entities.Calculation;
using OilFieldPlatform.Domain.Projections.Calculation;
using OilFieldPlatform.Infrastructure.Repositories.ABC;
using NHibernate;
using NHibernate.Linq;

namespace OilFieldPlatform.Infrastructure.Repositories.Calculation;

/// <summary>
/// Репозиторий чтения для CalcProjectEntity (расчётный проект).
/// </summary>
public class CalcProjectReadRepository : ABCRepository<CalcProjectEntity>
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="session">Сессия NHibernate.</param>
    /// <param name="logger">Логгер (опционально).</param>
    public CalcProjectReadRepository(ISession session, ILogger<CalcProjectReadRepository>? logger = null) : base(session, logger)
    {
    }

    private IQueryable<CalcProjectEntity> BuildByOilFieldId(long oilFieldId)
    {
        return _session.Query<CalcProjectEntity>()
            .Where(p => p.OilField!.Id == oilFieldId);
    }

    /// <summary>
    /// Получить проекты по месторождению.
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <returns>Список проектов.</returns>
    public IEnumerable<CalcProjectEntity> GetByOilFieldId(long oilFieldId)
    {
        _logger.LogDebug("Getting calc projects by oil field Id: {OilFieldId}", oilFieldId);
        var result = BuildByOilFieldId(oilFieldId).ToList();
        _logger.LogDebug("Found {Count} calc projects for oil field Id: {OilFieldId}", result.Count, oilFieldId);
        return result;
    }

    /// <summary>
    /// Получить проекты по месторождению (асинхронно).
    /// </summary>
    /// <param name="oilFieldId">Id месторождения.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список проектов.</returns>
    public async Task<IEnumerable<CalcProjectEntity>> GetByOilFieldIdAsync(long oilFieldId, CancellationToken ct = default)
    {
        _logger.LogDebug("Getting calc projects by oil field Id async: {OilFieldId}", oilFieldId);
        var result = await BuildByOilFieldId(oilFieldId).ToListAsync(ct);
        _logger.LogDebug("Found {Count} calc projects for oil field Id: {OilFieldId} async", result.Count, oilFieldId);
        return result;
    }

    private IQueryable<CalcProjectProjection> BuildProjectionsQuery()
    {
        return _session.Query<CalcProjectEntity>()
            .Select(e => new CalcProjectProjection { Id = e.Id!.Value, Name = e.Name });
    }

    /// <summary>
    /// Получить проекции всех расчётных проектов.
    /// </summary>
    /// <returns>Список проекций проектов.</returns>
    public IEnumerable<CalcProjectProjection> GetProjections()
    {
        _logger.LogDebug("Getting calc project projections");
        var result = BuildProjectionsQuery().ToList();
        _logger.LogDebug("Retrieved {Count} calc project projections", result.Count);
        return result;
    }

    /// <summary>
    /// Получить проекции всех расчётных проектов (асинхронно).
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список проекций проектов.</returns>
    public async Task<IEnumerable<CalcProjectProjection>> GetProjectionsAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Getting calc project projections async");
        var result = await BuildProjectionsQuery().ToListAsync(ct);
        _logger.LogDebug("Retrieved {Count} calc project projections async", result.Count);
        return result;
    }
}
